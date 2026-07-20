INSERT INTO StudentAdmissions (
    FullName, Gender, DateOfBirth, Email, PhoneNum, 
    StreetAddress, City, StateProvince, PostalCode, 
    PrevSchoolName, GPA, GraduationYear, CourseID, ApplicationStatus
)
VALUES 
('Alice Smith', 'Female', '2006-05-14', 'alice.smith@email.com', '555-0199', 
 '123 Maple St', 'Seattle', 'WA', '98101', 'West High School', 3.85, 2024, 1, 'Approved'),
 
('Bob Jones', 'Male', '2005-11-23', 'bob.jones@email.com', '555-0144', 
 '456 Oak Road', 'Austin', 'TX', '73301', 'Gateway Academy', 3.40, 2023, 2, 'Pending'),

('Carlos Mendez', 'Other', '2004-02-10', 'carlos.m@email.com', '555-0177', 
 '789 Pine Ave', 'Miami', 'FL', '33101', 'Riverview High', 2.95, 2022, 1, 'Approved'),

('Diana Prince', 'Female', '2007-01-15', 'diana.p@email.com', '555-0122', 
 '101 Skyline Dr', 'Denver', 'CO', '80201', 'Peak Prep', 4.00, 2025, 3, 'Approved');
GO
