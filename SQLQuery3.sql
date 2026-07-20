-- This will FAIL because the student is under 16 years old (Violates CHK_Student_Age)
INSERT INTO StudentAdmissions (FullName, Gender, DateOfBirth, Email, PhoneNum, StreetAddress, City, StateProvince, PostalCode, PrevSchoolName, GPA, GraduationYear, CourseID)
VALUES ('Young Student', 'Male', '2015-01-01', 'young@email.com', '555-9999', '123 St', 'City', 'ST', '12345', 'School', 3.50, 2026, 1);

-- This will FAIL because a GPA of 4.50 is out of bounds (Violates CHK_Student_GPA)
INSERT INTO StudentAdmissions (FullName, Gender, DateOfBirth, Email, PhoneNum, StreetAddress, City, StateProvince, PostalCode, PrevSchoolName, GPA, GraduationYear, CourseID)
VALUES ('Smart Student', 'Male', '2003-01-01', 'smart@email.com', '555-8888', '123 St', 'City', 'ST', '12345', 'School', 4.50, 2024, 1);
