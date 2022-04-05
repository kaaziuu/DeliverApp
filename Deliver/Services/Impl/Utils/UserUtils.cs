﻿using Microsoft.EntityFrameworkCore;
using Models.Db;
using Repository.Repository.Interface;
using Services.Interface.Utils;

namespace Services.Impl.Utils;

public class UserUtils : IUserUtils
{
    private readonly IUserRepository _userRepository;

    public UserUtils(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public async Task<User?> GetById(long id) =>
        await _userRepository
            .GetAll()
            .Include(x => x.UserRole)
            .ThenInclude(y => y.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
}
