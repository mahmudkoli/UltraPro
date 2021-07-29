using Microsoft.EntityFrameworkCore.Migrations;

namespace UltraPro.API.Migrations
{
    public partial class ApplicationLogAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var applicationLogs = @"CREATE TABLE [dbo].[ApplicationLogs](  
                                        [Id] [int] IDENTITY(1,1) NOT NULL,  
                                        [Message] [nvarchar](max) NULL,  
                                        [MessageTemplate] [nvarchar](max) NULL,  
                                        [Level] [nvarchar](128) NULL,  
                                        [TimeStamp] [datetimeoffset](7) NOT NULL,  
                                        [Exception] [nvarchar](max) NULL,  
                                        [Properties] [xml] NULL,  
                                        [LogEvent] [nvarchar](max) NULL,  
                                        [UserName] [nvarchar](200) NULL,  
                                        [IP] [varchar](200) NULL,  
                                     CONSTRAINT [PK_ApplicationLogs] PRIMARY KEY CLUSTERED   
                                    (  
                                        [Id] ASC  
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]  
                                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]  ";
            migrationBuilder.Sql(applicationLogs);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var applicationLogs = @"DROP TABLE [dbo].[ApplicationLogs]";
            migrationBuilder.Sql(applicationLogs);
        }
    }
}
