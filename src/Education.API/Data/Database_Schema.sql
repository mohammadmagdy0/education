-- 1. جدول المستخدمين الرئيسي (يضم الحسابات الأساسية لجميع الأدوار)
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL, -- لتخزين كلمة المرور مشفرة
    UserRole NVARCHAR(30) NOT NULL, -- (Admin, Teacher, Student, Parent, Support, Distributor)
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- 2. جدول تفاصيل الطلاب (علاقة 1 إلى 1 مع جدول المستخدمين)
CREATE TABLE Students (
    StudentId INT PRIMARY KEY FOREIGN KEY REFERENCES Users(UserId),
    ParentId INT NULL FOREIGN KEY REFERENCES Users(UserId), -- ربط الطالب بولي أمره
    AcademicYear NVARCHAR(50) NULL -- (مثل: Freshman, Level 01)
);

-- 3. جدول تفاصيل المعلمين
CREATE TABLE Teachers (
    TeacherId INT PRIMARY KEY FOREIGN KEY REFERENCES Users(UserId),
    Specialization NVARCHAR(100) NULL,
    Bio NVARCHAR(MAX) NULL
);

-- 4. جدول المحافظ المالية (للطبّاق، المعلمين، والموزعين)
CREATE TABLE Wallets (
    WalletId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT UNIQUE FOREIGN KEY REFERENCES Users(UserId) NOT NULL,
    Balance DECIMAL(18, 2) DEFAULT 0.00 NOT NULL,
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- 5. جدول كروت الشحن الذكية
CREATE TABLE ScratchCards (
    CardId INT IDENTITY(1,1) PRIMARY KEY,
    CardCode NVARCHAR(50) UNIQUE NOT NULL, -- الرمز السري للكارت
    Amount DECIMAL(18, 2) NOT NULL, -- قيمة الكارت المالية
    CreatedByAdminId INT FOREIGN KEY REFERENCES Users(UserId) NOT NULL, -- الأدمن اللي عمل الكارت
    AssignedToDistributorId INT NULL FOREIGN KEY REFERENCES Users(UserId), -- الموزع المستلم للكارت للبيع
    IsUsed BIT DEFAULT 0 NOT NULL,
    RedeemedByUserId INT NULL FOREIGN KEY REFERENCES Users(UserId), -- الطالب أو ولي الأمر اللي شحن الكارت
    UsedAt DATETIME NULL
);

-- 6. جدول سجل المعاملات المالية (للتدقيق ومتابعة حركة الأموال)
CREATE TABLE WalletTransactions (
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    WalletId INT FOREIGN KEY REFERENCES Wallets(WalletId) NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL, -- القيمة (موجبة للشحن، سالبة للشراء)
    TransactionType NVARCHAR(50) NOT NULL, -- (Recharge, CoursePurchase, Payout)
    Description NVARCHAR(255) NULL,
    TransactionDate DATETIME DEFAULT GETDATE()
);