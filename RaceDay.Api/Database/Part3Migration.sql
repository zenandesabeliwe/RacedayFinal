-- Execute once against the existing RaceDay database before using image upload endpoints.
IF COL_LENGTH('dbo.Events','bannerUrl') IS NULL ALTER TABLE dbo.Events ADD bannerUrl NVARCHAR(1024) NULL;
IF COL_LENGTH('dbo.Users','profilePictureUrl') IS NULL ALTER TABLE dbo.Users ADD profilePictureUrl NVARCHAR(1024) NULL;
