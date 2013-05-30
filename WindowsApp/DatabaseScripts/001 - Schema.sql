USE [master]
GO
/****** Object:  Database [klick_vending_machine]    Script Date: 5/30/2013 9:20:24 AM ******/
CREATE DATABASE [klick_vending_machine]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'klick_vending_machine', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\klick_vending_machine.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'klick_vending_machine_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\klick_vending_machine_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [klick_vending_machine] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [klick_vending_machine].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [klick_vending_machine] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [klick_vending_machine] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [klick_vending_machine] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [klick_vending_machine] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [klick_vending_machine] SET ARITHABORT OFF 
GO
ALTER DATABASE [klick_vending_machine] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [klick_vending_machine] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [klick_vending_machine] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [klick_vending_machine] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [klick_vending_machine] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [klick_vending_machine] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [klick_vending_machine] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [klick_vending_machine] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [klick_vending_machine] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [klick_vending_machine] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [klick_vending_machine] SET  DISABLE_BROKER 
GO
ALTER DATABASE [klick_vending_machine] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [klick_vending_machine] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [klick_vending_machine] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [klick_vending_machine] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [klick_vending_machine] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [klick_vending_machine] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [klick_vending_machine] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [klick_vending_machine] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [klick_vending_machine] SET  MULTI_USER 
GO
ALTER DATABASE [klick_vending_machine] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [klick_vending_machine] SET DB_CHAINING OFF 
GO
ALTER DATABASE [klick_vending_machine] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [klick_vending_machine] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [klick_vending_machine]
GO
/****** Object:  Table [dbo].[CardScan]    Script Date: 5/30/2013 9:20:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CardScan](
	[CardScanID] [bigint] IDENTITY(1,1) NOT NULL,
	[CardID] [varchar](100) NOT NULL,
	[ScanDate] [datetime] NOT NULL,
 CONSTRAINT [PK_CardScan] PRIMARY KEY CLUSTERED 
(
	[CardScanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CardScanResult]    Script Date: 5/30/2013 9:20:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CardScanResult](
	[CardScanResultID] [bigint] NOT NULL,
	[CardScanID] [bigint] NOT NULL,
	[CardBatch] [varchar](100) NULL,
	[CardNumber] [varchar](100) NULL,
	[CardFirstName] [varchar](100) NULL,
	[CardLastName] [varchar](100) NULL,
	[ResultDate] [datetime] NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[_created] [datetime] NOT NULL,
 CONSTRAINT [PK_CardScanResult] PRIMARY KEY CLUSTERED 
(
	[CardScanResultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Error]    Script Date: 5/30/2013 9:20:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Error](
	[ErrorID] [bigint] IDENTITY(1,1) NOT NULL,
	[EntityName] [varchar](50) NULL,
	[EntityID] [bigint] NULL,
	[ErrorMessage] [text] NULL,
	[ErrorStackTrace] [text] NULL,
	[ChildErrorMessage] [text] NULL,
	[ChildErrorStackTrace] [text] NULL,
	[_created] [datetime] NOT NULL,
 CONSTRAINT [PK_Error] PRIMARY KEY CLUSTERED 
(
	[ErrorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VendingRequest]    Script Date: 5/30/2013 9:20:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VendingRequest](
	[VendingRequestID] [bigint] IDENTITY(1,1) NOT NULL,
	[CardScanResultID] [bigint] NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[Coordinates] [varchar](100) NULL,
	[X] [int] NULL,
	[Y] [int] NULL,
	[Status] [varchar](50) NULL,
	[VendStartDate] [datetime] NULL,
	[VendEndDate] [datetime] NULL,
	[_created] [datetime] NOT NULL,
 CONSTRAINT [PK_VendingRequest] PRIMARY KEY CLUSTERED 
(
	[VendingRequestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[CardScan] ADD  CONSTRAINT [DF_CardScan_ScanDate]  DEFAULT (getdate()) FOR [ScanDate]
GO
ALTER TABLE [dbo].[CardScanResult] ADD  CONSTRAINT [DF_CardScanResult__created]  DEFAULT (getdate()) FOR [_created]
GO
ALTER TABLE [dbo].[Error] ADD  CONSTRAINT [DF_Error__created]  DEFAULT (getdate()) FOR [_created]
GO
ALTER TABLE [dbo].[VendingRequest] ADD  CONSTRAINT [DF_VendingRequest__created]  DEFAULT (getdate()) FOR [_created]
GO
ALTER TABLE [dbo].[CardScanResult]  WITH CHECK ADD  CONSTRAINT [FK_CardScanResult_CardScan] FOREIGN KEY([CardScanID])
REFERENCES [dbo].[CardScan] ([CardScanID])
GO
ALTER TABLE [dbo].[CardScanResult] CHECK CONSTRAINT [FK_CardScanResult_CardScan]
GO
ALTER TABLE [dbo].[VendingRequest]  WITH CHECK ADD  CONSTRAINT [FK_VendingRequest_CardScanResult] FOREIGN KEY([CardScanResultID])
REFERENCES [dbo].[CardScanResult] ([CardScanResultID])
GO
ALTER TABLE [dbo].[VendingRequest] CHECK CONSTRAINT [FK_VendingRequest_CardScanResult]
GO
ALTER TABLE [dbo].[CardScanResult]  WITH CHECK ADD  CONSTRAINT [CK_CardScanResult_Status] CHECK  (([Status]='valid' OR [Status]='invalid' OR [Status]='failed'))
GO
ALTER TABLE [dbo].[CardScanResult] CHECK CONSTRAINT [CK_CardScanResult_Status]
GO
ALTER TABLE [dbo].[VendingRequest]  WITH CHECK ADD  CONSTRAINT [CK_VendingRequest] CHECK  (([Status]='failed' OR [Status]='vending' OR [Status]='complete'))
GO
ALTER TABLE [dbo].[VendingRequest] CHECK CONSTRAINT [CK_VendingRequest]
GO
USE [master]
GO
ALTER DATABASE [klick_vending_machine] SET  READ_WRITE 
GO
