-- 1. Create the School Database
CREATE DATABASE SchoolDB;
GO

-- Switch to the newly created database context
USE SchoolDB;
GO

-- 2. Create the Courses Master Table
CREATE TABLE Courses (
    CourseID INT IDENTITY(1,1),
    CourseCode VARCHAR(10) NOT NULL,
    CourseName VARCHAR(100) NOT NULL,
    Department VARCHAR(50) NOT NULL,
    Credits INT NOT NULL,
    
    CONSTRAINT PK_Courses PRIMARY KEY (CourseID),
    CONSTRAINT UQ_CourseCode UNIQUE (CourseCode),
    CONSTRAINT CHK_Course_Credits CHECK (Credits > 0 AND Credits <= 6)
);
GO

-- 3. Create the Student Admissions Table
CREATE TABLE StudentAdmissions (
    StudentID INT IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,
    Gender VARCHAR(15) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PhoneNum VARCHAR(20) NOT NULL,
    
    -- Address Fields
    StreetAddress VARCHAR(255) NOT NULL,
    City VARCHAR(50) NOT NULL,
    StateProvince VARCHAR(50) NOT NULL,
    PostalCode VARCHAR(15) NOT NULL,
    
    -- Academic Background Fields
    PrevSchoolName VARCHAR(150) NOT NULL,
    GPA DECIMAL(3,2) NOT NULL,
    GraduationYear INT NOT NULL,
    
    -- Admission Info Fields
    CourseID INT NOT NULL, -- Target Foreign Key
    AdmissionDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    ApplicationStatus VARCHAR(20) NOT NULL DEFAULT 'Pending',

    -- Table Constraints & Validation Rules
    CONSTRAINT PK_StudentAdmissions PRIMARY KEY (StudentID),
    CONSTRAINT UQ_Student_Email UNIQUE (Email),
    CONSTRAINT UQ_Student_Phone UNIQUE (PhoneNum),
    
    -- Foreign Key Relationship
    CONSTRAINT FK_Student_Course FOREIGN KEY (CourseID) 
        REFERENCES Courses(CourseID) 
        ON DELETE NO ACTION 
        ON UPDATE CASCADE,
    
    -- Age Validation: Must be 16 or older (T-SQL DATEDIFF approach)
    CONSTRAINT CHK_Student_Age CHECK (DATEDIFF(YEAR, DateOfBirth, GETDATE()) >= 16),
    
    -- GPA Validation: Must be between 0.00 and 4.00
    CONSTRAINT CHK_Student_GPA CHECK (GPA >= 0.00 AND GPA <= 4.00),
    
    -- Status Validation: Restrict to specific values
    CONSTRAINT CHK_Application_Status CHECK (ApplicationStatus IN ('Pending', 'Approved', 'Rejected')),
    
    -- Email Format Validation: T-SQL pattern matching
    CONSTRAINT CHK_Email_Format CHECK (Email LIKE '%_@__%.__%')
);
GO
