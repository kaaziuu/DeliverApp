ALTER TABLE Cars
	ADD CompanyId BIGINT FOREIGN KEY REFERENCES "Company"(Id);