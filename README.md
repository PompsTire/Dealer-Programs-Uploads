# Dealer-Programs-Uploads

Replacement for separate programs for each dealer

"Dealer Programs Uploads.exe" is the new program that handles all of the data retrieval and uploads for each dealer program.

It replaces all of the other programs that assembled and uploaded sales data for each of the dealer programs.
Previous programs were run through Windows Task Scheduler on GBSQL01v2.

The new program is scheduled and ran by GBSQL01v2 scheduled job: "Dealer Programs File Upload". It is a console program that uses command line parameters to run the data retrieval and file upload to each dealer.

These are the command line parameters (not case sensitive):
  GOODYEAR
  PIRELLI
  HANKOOK
  BFSSELLOUT
  BFSTIER1
  GFK (Added 1.9.2020)

Example: To run the program for Hankook, right click on a shortcut to the exe. In the target field, add a space then the command line parameter after the path like so: "C:\Scheduled Tasks\DealerProgramsUploads\Dealer Programs Uploads.exe" hankook. Hit OK to save then double click the shortcut to run the program.

The old programs used internal, hard-coded sql queries to fetch the data. The new program uses stored procedures. When a request is made to submit data for a particular date range/dealer etc, we can just change the stored proc, instead of altering and recompiling the program.

The stored procs for each dealer (on GBSQL01v2):
Goodyear: Dealer_Programs.dbo.up_Goodyear_DailyUploads
Pirelli:  Dealer_Programs.dbo.up_Pirelli_DailyUploads
Hankook:  Dealer_Programs.dbo.up_Hankook_DailyUploads
BFSSELLOUT: Dealer_Programs.dbo.up_BFSSellout_DailyUploads
BFSTIRE1: Dealer_Programs.dbo.up_BFSTier1_DailyUploads
GFK: Dealer_Programs.dbo.up_DailyUploads_GFK

1.9.2020  -------------------------------------------------------------------------------------
Added new Stored Proc Dealer_Programs.dbo.up_DailyUploads_GFK

Changed stored proc names:
Goodyear: Dealer_Programs.dbo.up_Goodyear_DailyUploads --> up_DailyUploads_Goodyear
Pirelli:  Dealer_Programs.dbo.up_Pirelli_DailyUploads  --> up_DailyUploads_Pirelli
Hankook:  Dealer_Programs.dbo.up_Hankook_DailyUploads  --> up_DailyUploads_Hankook
BFSSELLOUT: Dealer_Programs.dbo.up_BFSSellout_DailyUploads  --> up_DailyUploads_BFSSellout
BFSTIRE1: Dealer_Programs.dbo.up_BFSTier1_DailyUploads  --> up_DailyUploads_BFSTier1
-----------------------------------------------------------------------------------------------
Settings: FTP information and file paths are stored in database: [Dealer_Programs].[dbo].[tb_DealerPrograms_DealerAppSettings]