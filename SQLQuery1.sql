-- Create tables
Create database RentAPlaceDB;
use RentAPlaceDB;
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Owner','Renter')),
    Phone NVARCHAR(30) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE Properties (
    Id INT IDENTITY PRIMARY KEY,
    OwnerId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    Address NVARCHAR(300) NULL,
    City NVARCHAR(100) NOT NULL,
    [State] NVARCHAR(100) NULL,
    Country NVARCHAR(100) NOT NULL,
    Latitude DECIMAL(9,6) NULL,
    Longitude DECIMAL(9,6) NULL,
    PricePerNight DECIMAL(10,2) NOT NULL,
    PropertyType NVARCHAR(50) NOT NULL,
    MaxGuests INT NOT NULL DEFAULT 1,
    Bedrooms INT NOT NULL DEFAULT 1,
    Bathrooms INT NOT NULL DEFAULT 1,
    IsActive BIT NOT NULL DEFAULT 1,
    AverageRating DECIMAL(3,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Properties_Users_OwnerId FOREIGN KEY (OwnerId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE Amenities (
    Id INT IDENTITY PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE PropertyAmenities (
    PropertyId INT NOT NULL,
    AmenityId INT NOT NULL,
    PRIMARY KEY (PropertyId, AmenityId),
    CONSTRAINT FK_PropertyAmenities_Properties FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PropertyAmenities_Amenities FOREIGN KEY (AmenityId) REFERENCES Amenities(Id) ON DELETE CASCADE
);

CREATE TABLE PropertyImages (
    Id INT IDENTITY PRIMARY KEY,
    PropertyId INT NOT NULL,
    ImageUrl NVARCHAR(400) NOT NULL,
    CONSTRAINT FK_PropertyImages_Properties FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE CASCADE
);

CREATE TABLE Reservations (
    Id INT IDENTITY PRIMARY KEY,
    PropertyId INT NOT NULL,
    RenterId UNIQUEIDENTIFIER NOT NULL,
    CheckIn DATE NOT NULL,
    CheckOut DATE NOT NULL,
    Guests INT NOT NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Pending','Confirmed','Cancelled')) DEFAULT 'Pending',
    TotalPrice DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Reservations_Properties FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Reservations_Renter FOREIGN KEY (RenterId) REFERENCES Users(Id) ON DELETE NO ACTION
);

-- Optional tables for future use; not needed in UI for now
CREATE TABLE Messages (
    Id INT IDENTITY PRIMARY KEY,
    PropertyId INT NOT NULL,
    SenderId UNIQUEIDENTIFIER NOT NULL,
    ReceiverId UNIQUEIDENTIFIER NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    SentAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsRead BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Messages_Properties FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE CASCADE
);

CREATE TABLE Reviews (
    Id INT IDENTITY PRIMARY KEY,
    PropertyId INT NOT NULL,
    RenterId UNIQUEIDENTIFIER NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Reviews_Properties FOREIGN KEY (PropertyId) REFERENCES Properties(Id) ON DELETE CASCADE
);

-- Seed basic amenities
INSERT INTO Amenities([Name]) VALUES
('wifi'),('Air Conditioning'),('Heating'),('Kitchen'),('Washer'),('Dryer'),('Free Parking'),('Pool'),('Gym'),('Sea View');

ALTER TABLE Users
ADD UpdatedAt DATETIME2 NULL;
SELECT * FROM Users;

ALTER TABLE Properties
ADD UpdatedAt DATETIME NULL;

SELECT * FROM Properties;

SELECT * FROM Reservations;


ALTER TABLE Reservations
ADD UpdatedAt DATETIME NOT NULL DEFAULT GETDATE();

SELECT * FROM Messages;

DELETE FROM Messages
WHERE Id=8;

DELETE FROM Reservations
WHERE PropertyId=50 AND Id=7;

SELECT * FROM Properties;

SELECT * FROM Reviews;

ALTER TABLE Reviews
ADD UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
ALTER TABLE Reviews
ALTER COLUMN UpdatedAt DATETIME2 NULL;

SELECT * FROM PropertyAmenities;


SELECT * FROM Users;
SELECT * FROM Properties;
SELECT * FROM PropertyAmenities;
SELECT * FROM PropertyImages;
SELECT * FROM Reservations;
SELECT * FROM Messages;
SELECT * FROM Reviews;
SELECT *FROM Amenities;

delete from PropertyImages
where id=2;

delete from Properties
where id=9;

delete from PropertyImages
where id=6;

delete from PropertyImages
where id=5;

