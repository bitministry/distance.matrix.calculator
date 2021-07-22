
-- 1. REPLACE "***DISCO_ver***" with Database name everywhere in this script


USE [master]
GO

/****** Object:  Database [***DISCO_ver***]    Script Date: 08/14/2014 11:50:55 ******/
CREATE DATABASE [***DISCO_ver***] ON  PRIMARY 
( NAME = N'***DISCO_ver***', FILENAME = N'G:\Data\Shared\***DISCO_ver***.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'***DISCO_ver***_log', FILENAME = N'G:\TLogs\Shared\***DISCO_ver***_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [***DISCO_ver***] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [***DISCO_ver***].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [***DISCO_ver***] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET ARITHABORT OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [***DISCO_ver***] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [***DISCO_ver***] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [***DISCO_ver***] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET  DISABLE_BROKER 
GO
ALTER DATABASE [***DISCO_ver***] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [***DISCO_ver***] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [***DISCO_ver***] SET  READ_WRITE 
GO
ALTER DATABASE [***DISCO_ver***] SET RECOVERY FULL 
GO
ALTER DATABASE [***DISCO_ver***] SET  MULTI_USER 
GO
ALTER DATABASE [***DISCO_ver***] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [***DISCO_ver***] SET DB_CHAINING OFF 
GO


USE [***DISCO_ver***] 


/****** Object:  Table [dbo].[Contact]    Script Date: 08/14/2014 11:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact](
	[ContactId] [int] IDENTITY(1,1) NOT NULL,
	[ContactType] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](33) NULL,
	[Email] [nvarchar](66) NULL,
	[AddressesModified] [datetime] NULL,
 CONSTRAINT [PK_dbo.Contact] PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[uspGetMatchingDistanceComparison]    Script Date: 08/14/2014 11:52:08 ******/
SET ANSI_NULLS ON
GO

/****** Object:  Table [dbo].[Address]    Script Date: 08/14/2014 11:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[AddressId] [int] IDENTITY(1,1) NOT NULL,
	[PostCode] [nvarchar](18) NOT NULL,
	[CountryCodeIso3] [nvarchar](3) NULL,
	[Line1] [nvarchar](max) NULL,
	[Phone] [nvarchar](33) NULL,
	[Email] [nvarchar](66) NULL,
	[Lat] [float] NULL,
	[Lng] [float] NULL,
	[ContactId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Address] PRIMARY KEY CLUSTERED 
(
	[AddressId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DistanceComparison]    Script Date: 08/14/2014 11:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DistanceComparison](
	[DistanceComparisonId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.DistanceComparison] PRIMARY KEY CLUSTERED 
(
	[DistanceComparisonId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CompetitorsIncludedToDistanceComparison]    Script Date: 08/14/2014 11:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompetitorsIncludedToDistanceComparison](
	[DistanceComparisonId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.CompetitorsIncludedToDistanceComparison] PRIMARY KEY CLUSTERED 
(
	[DistanceComparisonId] ASC,
	[ContactId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ComparisonReports]    Script Date: 08/14/2014 11:52:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ComparisonReports](
	[ComparisonReportId] [int] IDENTITY(1,1) NOT NULL,
	[Serialized_Contact_Customer] [varbinary](max) NULL,
	[Serialized_ICollectionFetcherLogForTarget] [varbinary](max) NULL,
	[Created] [datetime] NOT NULL,
	[DistanceComparisonId] [int] NOT NULL,
	ProcessedElements int not null, 
	GoogleStoppedResponding bit not null, 
 CONSTRAINT [PK_dbo.ComparisonReports] PRIMARY KEY CLUSTERED 
(
	[ComparisonReportId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF__DistanceC__Custo__6754599E]    Script Date: 08/14/2014 11:52:01 ******/
ALTER TABLE [dbo].[DistanceComparison] ADD  DEFAULT ((0)) FOR [CustomerId]
GO
/****** Object:  ForeignKey [FK_dbo.Address_dbo.Contact_ContactId]    Script Date: 08/14/2014 11:52:01 ******/
ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Address_dbo.Contact_ContactId] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([ContactId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_dbo.Address_dbo.Contact_ContactId]
GO
/****** Object:  ForeignKey [FK_dbo.ComparisonReports_dbo.DistanceComparison_DistanceComparisonId]    Script Date: 08/14/2014 11:52:01 ******/
ALTER TABLE [dbo].[ComparisonReports]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ComparisonReports_dbo.DistanceComparison_DistanceComparisonId] FOREIGN KEY([DistanceComparisonId])
REFERENCES [dbo].[DistanceComparison] ([DistanceComparisonId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ComparisonReports] CHECK CONSTRAINT [FK_dbo.ComparisonReports_dbo.DistanceComparison_DistanceComparisonId]
GO
/****** Object:  ForeignKey [FK_dbo.CompetitorsIncludedToDistanceComparison_dbo.Contact_ContactId]    Script Date: 08/14/2014 11:52:01 ******/
ALTER TABLE [dbo].[CompetitorsIncludedToDistanceComparison]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CompetitorsIncludedToDistanceComparison_dbo.Contact_ContactId] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([ContactId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CompetitorsIncludedToDistanceComparison] CHECK CONSTRAINT [FK_dbo.CompetitorsIncludedToDistanceComparison_dbo.Contact_ContactId]
GO
/****** Object:  ForeignKey [FK_dbo.CompetitorsIncludedToDistanceComparison_dbo.DistanceComparison_DistanceComparisonId]    Script Date: 08/14/2014 11:52:01 ******/
ALTER TABLE [dbo].[CompetitorsIncludedToDistanceComparison]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CompetitorsIncludedToDistanceComparison_dbo.DistanceComparison_DistanceComparisonId] FOREIGN KEY([DistanceComparisonId])
REFERENCES [dbo].[DistanceComparison] ([DistanceComparisonId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CompetitorsIncludedToDistanceComparison] CHECK CONSTRAINT [FK_dbo.CompetitorsIncludedToDistanceComparison_dbo.DistanceComparison_DistanceComparisonId]
GO
/****** Object:  ForeignKey [FK_dbo.DistanceComparison_dbo.Contact_CustomerId]    Script Date: 08/14/2014 11:52:01 ******/
ALTER TABLE [dbo].[DistanceComparison]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DistanceComparison_dbo.Contact_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[DistanceComparison] CHECK CONSTRAINT [FK_dbo.DistanceComparison_dbo.Contact_CustomerId]
GO





