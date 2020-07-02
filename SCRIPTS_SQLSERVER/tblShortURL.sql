CREATE TABLE [dbo].[tblShortURL](
	[idShortURL] [int] IDENTITY(1,1) NOT NULL,
	[OriginalURL] [varchar](50) NULL,
	[ShortURL] [varchar](50) NULL,
	[alias] [varchar](50) NULL,
	[isAutomaticAlias] [int] NULL,
	[dtCreation] [datetime] NULL,
 CONSTRAINT [PK_tblShortURL] PRIMARY KEY CLUSTERED 
(
	[idShortURL] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]