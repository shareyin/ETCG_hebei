USE [ETCF]
GO
/****** Object:  Table [dbo].[OBUInfo]    Script Date: 07/31/2017 15:50:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OBUInfo](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OBUPlateColor] [varchar](50) NULL,
	[OBUPlateNum] [varchar](50) NULL,
	[OBUMac] [varchar](50) NULL,
	[OBUY] [varchar](50) NULL,
	[OBUCarType] [varchar](50) NULL,
	[TradeTime] [varchar](50) NULL,
	[RandCode] [varchar](50) NULL,
	[OBUCarLength] [varchar](50) NULL,
	[OBUCarHigh] [varchar](50) NULL,
 CONSTRAINT [PK_OBUInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[JGInfo]    Script Date: 07/31/2017 15:50:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[JGInfo](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[JGLength] [varchar](50) NULL,
	[JGWide] [varchar](50) NULL,
	[JGCarType] [varchar](50) NULL,
	[CamPlateNum] [varchar](50) NULL,
	[ForceTime] [varchar](50) NULL,
	[Cambiao] [varchar](50) NULL,
	[CamPicPath] [varchar](200) NULL,
	[JGId] [varchar](50) NULL,
	[CamPlateColor] [varchar](50) NULL,
	[RandCode] [varchar](50) NULL,
 CONSTRAINT [PK_JGInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CarInfo]    Script Date: 07/31/2017 15:50:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CarInfo](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[JGLength] [varchar](50) NULL,
	[JGWide] [varchar](50) NULL,
	[JGCarType] [varchar](50) NULL,
	[ForceTime] [varchar](50) NULL,
	[CamPlateColor] [varchar](50) NULL,
	[CamPlateNum] [varchar](50) NULL,
	[Cambiao] [varchar](50) NULL,
	[CamPicPath] [varchar](200) NULL,
	[JGId] [varchar](50) NULL,
	[OBUPlateColor] [varchar](50) NULL,
	[OBUPlateNum] [varchar](50) NULL,
	[OBUMac] [varchar](50) NULL,
	[OBUY] [varchar](50) NULL,
	[OBUCarLength] [varchar](50) NULL,
	[OBUCarHigh] [varchar](50) NULL,
	[OBUCarType] [varchar](50) NULL,
	[TradeTime] [varchar](50) NULL,
	[TradeState] [varchar](50) NULL,
	[RandCode] [varchar](50) NULL,
	[GetFunction] [varchar](50) NULL,
 CONSTRAINT [PK_CarInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
