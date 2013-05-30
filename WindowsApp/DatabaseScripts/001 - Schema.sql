USE [klick_vending_machine]
GO
/****** Object:  Table [dbo].[CardScan]    Script Date: 05/29/2013 23:59:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CardScan]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CardScan](
	[CardScanID] [bigint] IDENTITY(1,1) NOT NULL,
	[CardID] [varchar](100) NOT NULL,
	[ScanDate] [datetime] NOT NULL,
	[CardBatch] [varchar](100) NULL,
	[CardNumber] [varchar](100) NULL,
	[CardFirstName] [varchar](100) NULL,
	[CardLastName] [varchar](100) NULL,
	[ValidationDate] [datetime] NULL,
	[Status] [varchar](50) NULL,
	[_created] [datetime] NOT NULL,
 CONSTRAINT [PK_CardScan] PRIMARY KEY CLUSTERED 
(
	[CardScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VendingRequest]    Script Date: 05/29/2013 23:59:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VendingRequest]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[VendingRequest](
	[VendingRequestID] [bigint] IDENTITY(1,1) NOT NULL,
	[CardScanID] [bigint] NOT NULL,
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Error]    Script Date: 05/29/2013 23:59:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Error]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Error](
	[ErrorID] [bigint] IDENTITY(1,1) NOT NULL,
	[CardScanID] [bigint] NULL,
	[VendingRequestID] [bigint] NULL,
	[ErrorMessage] [text] NULL,
	[ErrorStackTrace] [text] NULL,
	[ChildErrorMessage] [text] NULL,
	[ChildErrorStackTrace] [text] NULL,
	[_created] [datetime] NOT NULL,
 CONSTRAINT [PK_Error] PRIMARY KEY CLUSTERED 
(
	[ErrorID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Default [DF_CardScan_Status]    Script Date: 05/29/2013 23:59:37 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_CardScan_Status]') AND parent_object_id = OBJECT_ID(N'[dbo].[CardScan]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_CardScan_Status]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[CardScan] ADD  CONSTRAINT [DF_CardScan_Status]  DEFAULT ('scanned') FOR [Status]
END


End
GO
/****** Object:  Default [DF_CardScan__created]    Script Date: 05/29/2013 23:59:37 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_CardScan__created]') AND parent_object_id = OBJECT_ID(N'[dbo].[CardScan]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_CardScan__created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[CardScan] ADD  CONSTRAINT [DF_CardScan__created]  DEFAULT (getdate()) FOR [_created]
END


End
GO
/****** Object:  Default [DF_VendingRequest__created]    Script Date: 05/29/2013 23:59:37 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_VendingRequest__created]') AND parent_object_id = OBJECT_ID(N'[dbo].[VendingRequest]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_VendingRequest__created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[VendingRequest] ADD  CONSTRAINT [DF_VendingRequest__created]  DEFAULT (getdate()) FOR [_created]
END


End
GO
/****** Object:  Default [DF_Error__created]    Script Date: 05/29/2013 23:59:37 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Error__created]') AND parent_object_id = OBJECT_ID(N'[dbo].[Error]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Error__created]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Error] ADD  CONSTRAINT [DF_Error__created]  DEFAULT (getdate()) FOR [_created]
END


End
GO
/****** Object:  Check [CK_CardScan]    Script Date: 05/29/2013 23:59:37 ******/
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_CardScan]') AND parent_object_id = OBJECT_ID(N'[dbo].[CardScan]'))
ALTER TABLE [dbo].[CardScan]  WITH CHECK ADD  CONSTRAINT [CK_CardScan] CHECK  (([Status]='scanned' OR [Status]='valid' OR [Status]='invalid' OR [Status]='failed'))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_CardScan]') AND parent_object_id = OBJECT_ID(N'[dbo].[CardScan]'))
ALTER TABLE [dbo].[CardScan] CHECK CONSTRAINT [CK_CardScan]
GO
/****** Object:  Check [CK_VendingRequest]    Script Date: 05/29/2013 23:59:37 ******/
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_VendingRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[VendingRequest]'))
ALTER TABLE [dbo].[VendingRequest]  WITH CHECK ADD  CONSTRAINT [CK_VendingRequest] CHECK  (([Status]='failed' OR [Status]='vending' OR [Status]='complete'))
GO
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_VendingRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[VendingRequest]'))
ALTER TABLE [dbo].[VendingRequest] CHECK CONSTRAINT [CK_VendingRequest]
GO
/****** Object:  ForeignKey [FK_VendingRequest_CardScan]    Script Date: 05/29/2013 23:59:37 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VendingRequest_CardScan]') AND parent_object_id = OBJECT_ID(N'[dbo].[VendingRequest]'))
ALTER TABLE [dbo].[VendingRequest]  WITH CHECK ADD  CONSTRAINT [FK_VendingRequest_CardScan] FOREIGN KEY([CardScanID])
REFERENCES [dbo].[CardScan] ([CardScanID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VendingRequest_CardScan]') AND parent_object_id = OBJECT_ID(N'[dbo].[VendingRequest]'))
ALTER TABLE [dbo].[VendingRequest] CHECK CONSTRAINT [FK_VendingRequest_CardScan]
GO
/****** Object:  ForeignKey [FK_Error_CardScan]    Script Date: 05/29/2013 23:59:37 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Error_CardScan]') AND parent_object_id = OBJECT_ID(N'[dbo].[Error]'))
ALTER TABLE [dbo].[Error]  WITH CHECK ADD  CONSTRAINT [FK_Error_CardScan] FOREIGN KEY([CardScanID])
REFERENCES [dbo].[CardScan] ([CardScanID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Error_CardScan]') AND parent_object_id = OBJECT_ID(N'[dbo].[Error]'))
ALTER TABLE [dbo].[Error] CHECK CONSTRAINT [FK_Error_CardScan]
GO
/****** Object:  ForeignKey [FK_Error_VendingRequest]    Script Date: 05/29/2013 23:59:37 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Error_VendingRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[Error]'))
ALTER TABLE [dbo].[Error]  WITH CHECK ADD  CONSTRAINT [FK_Error_VendingRequest] FOREIGN KEY([VendingRequestID])
REFERENCES [dbo].[VendingRequest] ([VendingRequestID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Error_VendingRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[Error]'))
ALTER TABLE [dbo].[Error] CHECK CONSTRAINT [FK_Error_VendingRequest]
GO
