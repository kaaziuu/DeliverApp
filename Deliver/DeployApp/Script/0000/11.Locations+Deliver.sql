﻿ALTER TABLE Deliver
	ADD "From" BIGINT NOT NULL FOREIGN KEY REFERENCES Locations(Id),
	"To" BIGINT NOT NULL FOREIGN KEY REFERENCES Locations(Id);
