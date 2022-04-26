﻿using Integrations.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Models.Db;
using Models.Exceptions;
using Models.Integration;
using Models.Request.User;
using Models.Request.Utils;
using Models.Response.User;
using Repository.Repository.Interface;
using Services.Interface;
using Services.Interface.Utils;
using System.Security.Cryptography;

namespace Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyUtils _companyUtils;
        private readonly LoggedUser _loggedUser;
        private readonly IRoleUtils _roleUtils;
        private readonly IMailService _mailService;

        public UserService(
            IUserRepository userRepository,
            ICompanyUtils companyUtils,
            IOptions<LoggedUser> loggedUser,
            IRoleUtils roleUtils,
            IMailService mailService)
        {
            _userRepository = userRepository;
            _companyUtils = companyUtils;
            _loggedUser = loggedUser.Value;
            _roleUtils = roleUtils;
            _mailService = mailService;
        }

        public async Task<UserResponse> CreateUser(CreateUserRequest createUserRequest)
        {
            if (createUserRequest is null || !createUserRequest.IsValid)
            {
                throw new AppException(ErrorMessage.InvalidData);
            }

            if ((await _userRepository.GetAll().AnyAsync(x => x.Username == createUserRequest.Username)))
            {
                throw new AppException(ErrorMessage.UserExists);
            }

            if (!(await VerifyPerrmisionToAddUser(createUserRequest.CompanyHash)))
            {
                throw new AppException(ErrorMessage.InvalidRole);
            }

            var company = await _companyUtils.GetCompanyByHash(createUserRequest.CompanyHash);
            if (company is null)
            {
                throw new AppException(ErrorMessage.CompanyDoesntExists);
            }

            var newUser = await AddUserAndSendMail(createUserRequest.AsUser(), company);

            return new UserResponse
            {
                CompanyHash = company.Hash,
                CompanyName = company.Name,
                Hash = newUser.Hash,
                Email = newUser.Email,
                Name = newUser.Name,
                Surname = newUser.Surname,
                Username = newUser.Username,
            };
        }

        private async Task<bool> VerifyPerrmisionToAddUser(Guid hash)
        {
            var userCompany = await _companyUtils.GetUserCompany(_loggedUser.Id);
            return _roleUtils.HasPermissionToAddUser(new HasPermissionToAddUserRequest
            {
                LoggedUser = _loggedUser,
                LoggedUserCompany = userCompany,
                TargetCompanyHash = hash
            });
        }

        private async Task<User> AddUserAndSendMail(User user, Company company)
        {
            var password = Convert.ToBase64String(RandomNumberGenerator.GetBytes(5)).Replace("=", "");
            user.Hash = Guid.NewGuid();
            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            user.CompanyId = company.Id;

            await _userRepository.AddAsync(user);
            await SendWelcomeMail(user, password);
            return user;
        }

        private async Task SendWelcomeMail(User user, string password)
        {
            await _mailService.SendWelcomeMessage(new WelcomeMessageModel
            {
                Email = user.Email,
                Name = user.Name,
                Password = password,
                Surname = user.Surname,
                Username = user.Username
            });
        }

        public async Task AddRoleToUser(Guid userHash, List<long> RolesId)
        {
            var user = await _userRepository.GetByHashAsync(userHash);
            if (user is null)
            {
                throw new AppException(ErrorMessage.UserDosentExists);
            }
            await _roleUtils.AddRolesToUser(user, RolesId);
        }

        public async Task AddUserToCompany(Guid userHash, Guid companyHash)
        {
            var company = await _companyUtils.GetCompanyByHash(companyHash);
            var user = await _userRepository.GetByHashAsync(userHash);

            if (company is null)
            {
                throw new AppException(ErrorMessage.CompanyDoesntExists);
            }
            if (user is null)
            {
                throw new AppException(ErrorMessage.UserDosentExists);
            }

            user.Company = company;
            user.CompanyId = company.Id;

            await _userRepository.UpdateAsync(user);
        }
    }
}