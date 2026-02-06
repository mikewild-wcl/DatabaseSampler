CREATE TABLE [dbo].[PostcodeLookup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Postcode] [varchar](10) NOT NULL,
	[DistrictCode] [varchar](10) NOT NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[Location] [geography] NULL,
	[IsTerminated] [bit] NOT NULL,
	[TerminatedYear] [smallint] NULL,
	[TerminatedMonth] [smallint] NULL,
	[Created] [datetime2](7) NOT NULL DEFAULT (GETUTCDATE())
 CONSTRAINT [PK_PostcodeLookup] PRIMARY KEY CLUSTERED ([Id] ASC),
 INDEX [IX_PostcodeLookup_Postcode] NONCLUSTERED ([Postcode])
)