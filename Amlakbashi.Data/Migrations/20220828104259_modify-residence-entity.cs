using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class modifyresidenceentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.AdvertiseReports_dbo.Advertises_AdvertiseID",
                table: "AdvertiseReports");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Advertises_dbo.Advertises_Advertise_AdvertiseID",
                table: "Advertises");

            migrationBuilder.DropForeignKey(
                name: "FK_Advertises_Files_LicenseFileId",
                table: "Advertises");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Advertises_dbo.Files_PhotoID",
                table: "Advertises");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Advertises_dbo.Regions_Area",
                table: "Advertises");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Advertises_dbo.Regions_City",
                table: "Advertises");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Advertises_dbo.Regions_Province",
                table: "Advertises");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Advertises_dbo.Users_UserID",
                table: "Advertises");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Carts_dbo.Advertises_AdvertiseID",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Comments_dbo.Advertises_AdvertiseID",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.DiscountTables_dbo.Advertises_AdvertiseID",
                table: "DiscountTables");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.DynamicCategoryAdvertises_dbo.Advertises_Advertise_Id",
                table: "DynamicCategoryAdvertises");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.ExtrinsicReserves_dbo.Advertises_AdvertiseID",
                table: "ExtrinsicReserves");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.FileAdvertises_dbo.Advertises_Advertise_Id",
                table: "FileAdvertises");

            migrationBuilder.DropForeignKey(
                name: "FK_InstantReserveDate_Advertises_ResidenceId",
                table: "InstantReserveDate");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.OccupiedTables_dbo.Advertises_AdvertiseID",
                table: "OccupiedTables");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.PriceTables_dbo.Advertises_AdvertiseID",
                table: "PriceTables");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.ReportItems_dbo.Advertises_AdvertiseID",
                table: "ReportItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_Advertises_ResidenceId",
                table: "Reserves");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dbo.Advertises",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "ContactClick",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "IsContactAvailable",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "Overview",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "ParentAccType",
                table: "Advertises");

            migrationBuilder.RenameTable(
                name: "Advertises",
                newName: "Residences");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Residences",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "unixNorouzMinRequestDate",
                table: "Residences",
                newName: "MinReserveDateForNowruz");

            migrationBuilder.RenameColumn(
                name: "WebVisit",
                table: "Residences",
                newName: "View");

            migrationBuilder.RenameColumn(
                name: "TypeID",
                table: "Residences",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "TodayIsEmpty",
                table: "Residences",
                newName: "EmptyTonight");

            migrationBuilder.RenameColumn(
                name: "TidinessUserRating",
                table: "Residences",
                newName: "CleaningScore");

            migrationBuilder.RenameColumn(
                name: "SupportInfo",
                table: "Residences",
                newName: "SupportDescription");

            migrationBuilder.RenameColumn(
                name: "SingleBed",
                table: "Residences",
                newName: "SingleBedCount");

            migrationBuilder.RenameColumn(
                name: "Room",
                table: "Residences",
                newName: "RoomCount");

            migrationBuilder.RenameColumn(
                name: "RentPrice",
                table: "Residences",
                newName: "MonthlyPrice");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Residences",
                newName: "LocationType");

            migrationBuilder.RenameColumn(
                name: "Province",
                table: "Residences",
                newName: "ProvinceId");

            migrationBuilder.RenameColumn(
                name: "PhotoID",
                table: "Residences",
                newName: "MainPhotoId");

            migrationBuilder.RenameColumn(
                name: "OwnerMobile",
                table: "Residences",
                newName: "OwnerPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "NorouzPrice",
                table: "Residences",
                newName: "NowruzPrice");

            migrationBuilder.RenameColumn(
                name: "NorouzOverCapacityPrice",
                table: "Residences",
                newName: "NowruzExtraCapacityPrice");

            migrationBuilder.RenameColumn(
                name: "MoreThanCapacityPrice",
                table: "Residences",
                newName: "ExtraCapacityPrice");

            migrationBuilder.RenameColumn(
                name: "MoreThanCapacity",
                table: "Residences",
                newName: "ExtraCapacity");

            migrationBuilder.RenameColumn(
                name: "MinReserveDays",
                table: "Residences",
                newName: "MinReserveDuration");

            migrationBuilder.RenameColumn(
                name: "Metrazh",
                table: "Residences",
                newName: "BuildingArea");

            migrationBuilder.RenameColumn(
                name: "MaxReserveDays",
                table: "Residences",
                newName: "MaxReserveDuration");

            migrationBuilder.RenameColumn(
                name: "MaxInstantReserveStart",
                table: "Residences",
                newName: "MaxInstantReserveStartTimeInterval");

            migrationBuilder.RenameColumn(
                name: "LocationString",
                table: "Residences",
                newName: "RegionsPersianTitle");

            migrationBuilder.RenameColumn(
                name: "LastModifyDate",
                table: "Residences",
                newName: "LastModifiedDate");

            migrationBuilder.RenameColumn(
                name: "HolidayPikePrice",
                table: "Residences",
                newName: "PeakHolidayPrice");

            migrationBuilder.RenameColumn(
                name: "HideInCategory",
                table: "Residences",
                newName: "HideInSearch");

            migrationBuilder.RenameColumn(
                name: "EvidenceRequired",
                table: "Residences",
                newName: "RequiredEvidence");

            migrationBuilder.RenameColumn(
                name: "DoublesBed",
                table: "Residences",
                newName: "DoubleBedCount");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "Residences",
                newName: "UnitCount");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Residences",
                newName: "CityId");

            migrationBuilder.RenameColumn(
                name: "BlanketsAndMattresses",
                table: "Residences",
                newName: "BlanketAndMattressCount");

            migrationBuilder.RenameColumn(
                name: "AverageUserRating",
                table: "Residences",
                newName: "AverageUsersScore");

            migrationBuilder.RenameColumn(
                name: "Available",
                table: "Residences",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "Residences",
                newName: "AreaId");

            migrationBuilder.RenameColumn(
                name: "AllowSmoking",
                table: "Residences",
                newName: "Smoking");

            migrationBuilder.RenameColumn(
                name: "AllowPets",
                table: "Residences",
                newName: "Pets");

            migrationBuilder.RenameColumn(
                name: "AllowParty",
                table: "Residences",
                newName: "Party");

            migrationBuilder.RenameColumn(
                name: "Advertise_AdvertiseID",
                table: "Residences",
                newName: "ParentId");

            migrationBuilder.RenameColumn(
                name: "AdvertiseScore",
                table: "Residences",
                newName: "ResidenceScore");

            migrationBuilder.RenameIndex(
                name: "IX_UserID",
                table: "Residences",
                newName: "IX_Residences_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Province",
                table: "Residences",
                newName: "IX_Residences_ProvinceId");

            migrationBuilder.RenameIndex(
                name: "IX_PhotoID",
                table: "Residences",
                newName: "IX_Residences_MainPhotoId");

            migrationBuilder.RenameIndex(
                name: "IX_Advertises_LicenseFileId",
                table: "Residences",
                newName: "IX_Residences_LicenseFileId");

            migrationBuilder.RenameIndex(
                name: "IX_City",
                table: "Residences",
                newName: "IX_Residences_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_Area",
                table: "Residences",
                newName: "IX_Residences_AreaId");

            migrationBuilder.RenameIndex(
                name: "IX_Advertise_AdvertiseID",
                table: "Residences",
                newName: "IX_Residences_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Residences",
                table: "Residences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertiseReports_Residences_AdvertiseID",
                table: "AdvertiseReports",
                column: "AdvertiseID",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Residences_AdvertiseID",
                table: "Carts",
                column: "AdvertiseID",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Residences_AdvertiseID",
                table: "Comments",
                column: "AdvertiseID",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountTables_Residences_AdvertiseID",
                table: "DiscountTables",
                column: "AdvertiseID",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DynamicCategoryAdvertises_Residences_Advertise_Id",
                table: "DynamicCategoryAdvertises",
                column: "Advertise_Id",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtrinsicReserves_Residences_AdvertiseID",
                table: "ExtrinsicReserves",
                column: "AdvertiseID",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileAdvertises_Residences_Advertise_Id",
                table: "FileAdvertises",
                column: "Advertise_Id",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstantReserveDate_Residences_ResidenceId",
                table: "InstantReserveDate",
                column: "ResidenceId",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OccupiedTables_Residences_AdvertiseID",
                table: "OccupiedTables",
                column: "AdvertiseID",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceTables_Residences_AdvertiseID",
                table: "PriceTables",
                column: "AdvertiseID",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportItems_Residences_AdvertiseID",
                table: "ReportItems",
                column: "AdvertiseID",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Residences_ResidenceId",
                table: "Reserves",
                column: "ResidenceId",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Files_LicenseFileId",
                table: "Residences",
                column: "LicenseFileId",
                principalTable: "Files",
                principalColumn: "FileID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Files_MainPhotoId",
                table: "Residences",
                column: "MainPhotoId",
                principalTable: "Files",
                principalColumn: "FileID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Regions_AreaId",
                table: "Residences",
                column: "AreaId",
                principalTable: "Regions",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Regions_CityId",
                table: "Residences",
                column: "CityId",
                principalTable: "Regions",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Regions_ProvinceId",
                table: "Residences",
                column: "ProvinceId",
                principalTable: "Regions",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Residences_ParentId",
                table: "Residences",
                column: "ParentId",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Users_UserId",
                table: "Residences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertiseReports_Residences_AdvertiseID",
                table: "AdvertiseReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Residences_AdvertiseID",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Residences_AdvertiseID",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscountTables_Residences_AdvertiseID",
                table: "DiscountTables");

            migrationBuilder.DropForeignKey(
                name: "FK_DynamicCategoryAdvertises_Residences_Advertise_Id",
                table: "DynamicCategoryAdvertises");

            migrationBuilder.DropForeignKey(
                name: "FK_ExtrinsicReserves_Residences_AdvertiseID",
                table: "ExtrinsicReserves");

            migrationBuilder.DropForeignKey(
                name: "FK_FileAdvertises_Residences_Advertise_Id",
                table: "FileAdvertises");

            migrationBuilder.DropForeignKey(
                name: "FK_InstantReserveDate_Residences_ResidenceId",
                table: "InstantReserveDate");

            migrationBuilder.DropForeignKey(
                name: "FK_OccupiedTables_Residences_AdvertiseID",
                table: "OccupiedTables");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceTables_Residences_AdvertiseID",
                table: "PriceTables");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportItems_Residences_AdvertiseID",
                table: "ReportItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_Residences_ResidenceId",
                table: "Reserves");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Files_LicenseFileId",
                table: "Residences");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Files_MainPhotoId",
                table: "Residences");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Regions_AreaId",
                table: "Residences");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Regions_CityId",
                table: "Residences");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Regions_ProvinceId",
                table: "Residences");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Residences_ParentId",
                table: "Residences");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Users_UserId",
                table: "Residences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Residences",
                table: "Residences");

            migrationBuilder.RenameTable(
                name: "Residences",
                newName: "Advertises");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Advertises",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "View",
                table: "Advertises",
                newName: "WebVisit");

            migrationBuilder.RenameColumn(
                name: "UnitCount",
                table: "Advertises",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Advertises",
                newName: "TypeID");

            migrationBuilder.RenameColumn(
                name: "SupportDescription",
                table: "Advertises",
                newName: "SupportInfo");

            migrationBuilder.RenameColumn(
                name: "Smoking",
                table: "Advertises",
                newName: "AllowSmoking");

            migrationBuilder.RenameColumn(
                name: "SingleBedCount",
                table: "Advertises",
                newName: "SingleBed");

            migrationBuilder.RenameColumn(
                name: "RoomCount",
                table: "Advertises",
                newName: "Room");

            migrationBuilder.RenameColumn(
                name: "ResidenceScore",
                table: "Advertises",
                newName: "AdvertiseScore");

            migrationBuilder.RenameColumn(
                name: "RequiredEvidence",
                table: "Advertises",
                newName: "EvidenceRequired");

            migrationBuilder.RenameColumn(
                name: "RegionsPersianTitle",
                table: "Advertises",
                newName: "LocationString");

            migrationBuilder.RenameColumn(
                name: "ProvinceId",
                table: "Advertises",
                newName: "Province");

            migrationBuilder.RenameColumn(
                name: "Pets",
                table: "Advertises",
                newName: "AllowPets");

            migrationBuilder.RenameColumn(
                name: "PeakHolidayPrice",
                table: "Advertises",
                newName: "HolidayPikePrice");

            migrationBuilder.RenameColumn(
                name: "Party",
                table: "Advertises",
                newName: "AllowParty");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Advertises",
                newName: "Advertise_AdvertiseID");

            migrationBuilder.RenameColumn(
                name: "OwnerPhoneNumber",
                table: "Advertises",
                newName: "OwnerMobile");

            migrationBuilder.RenameColumn(
                name: "NowruzPrice",
                table: "Advertises",
                newName: "NorouzPrice");

            migrationBuilder.RenameColumn(
                name: "NowruzExtraCapacityPrice",
                table: "Advertises",
                newName: "NorouzOverCapacityPrice");

            migrationBuilder.RenameColumn(
                name: "MonthlyPrice",
                table: "Advertises",
                newName: "RentPrice");

            migrationBuilder.RenameColumn(
                name: "MinReserveDuration",
                table: "Advertises",
                newName: "MinReserveDays");

            migrationBuilder.RenameColumn(
                name: "MinReserveDateForNowruz",
                table: "Advertises",
                newName: "unixNorouzMinRequestDate");

            migrationBuilder.RenameColumn(
                name: "MaxReserveDuration",
                table: "Advertises",
                newName: "MaxReserveDays");

            migrationBuilder.RenameColumn(
                name: "MaxInstantReserveStartTimeInterval",
                table: "Advertises",
                newName: "MaxInstantReserveStart");

            migrationBuilder.RenameColumn(
                name: "MainPhotoId",
                table: "Advertises",
                newName: "PhotoID");

            migrationBuilder.RenameColumn(
                name: "LocationType",
                table: "Advertises",
                newName: "Region");

            migrationBuilder.RenameColumn(
                name: "LastModifiedDate",
                table: "Advertises",
                newName: "LastModifyDate");

            migrationBuilder.RenameColumn(
                name: "HideInSearch",
                table: "Advertises",
                newName: "HideInCategory");

            migrationBuilder.RenameColumn(
                name: "ExtraCapacityPrice",
                table: "Advertises",
                newName: "MoreThanCapacityPrice");

            migrationBuilder.RenameColumn(
                name: "ExtraCapacity",
                table: "Advertises",
                newName: "MoreThanCapacity");

            migrationBuilder.RenameColumn(
                name: "EmptyTonight",
                table: "Advertises",
                newName: "TodayIsEmpty");

            migrationBuilder.RenameColumn(
                name: "DoubleBedCount",
                table: "Advertises",
                newName: "DoublesBed");

            migrationBuilder.RenameColumn(
                name: "CleaningScore",
                table: "Advertises",
                newName: "TidinessUserRating");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "Advertises",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "BuildingArea",
                table: "Advertises",
                newName: "Metrazh");

            migrationBuilder.RenameColumn(
                name: "BlanketAndMattressCount",
                table: "Advertises",
                newName: "BlanketsAndMattresses");

            migrationBuilder.RenameColumn(
                name: "AverageUsersScore",
                table: "Advertises",
                newName: "AverageUserRating");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "Advertises",
                newName: "Area");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "Advertises",
                newName: "Available");

            migrationBuilder.RenameIndex(
                name: "IX_Residences_UserId",
                table: "Advertises",
                newName: "IX_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Residences_ProvinceId",
                table: "Advertises",
                newName: "IX_Province");

            migrationBuilder.RenameIndex(
                name: "IX_Residences_ParentId",
                table: "Advertises",
                newName: "IX_Advertise_AdvertiseID");

            migrationBuilder.RenameIndex(
                name: "IX_Residences_MainPhotoId",
                table: "Advertises",
                newName: "IX_PhotoID");

            migrationBuilder.RenameIndex(
                name: "IX_Residences_LicenseFileId",
                table: "Advertises",
                newName: "IX_Advertises_LicenseFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Residences_CityId",
                table: "Advertises",
                newName: "IX_City");

            migrationBuilder.RenameIndex(
                name: "IX_Residences_AreaId",
                table: "Advertises",
                newName: "IX_Area");

            migrationBuilder.AddColumn<int>(
                name: "ContactClick",
                table: "Advertises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsContactAvailable",
                table: "Advertises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Overview",
                table: "Advertises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OwnerID",
                table: "Advertises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentAccType",
                table: "Advertises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_dbo.Advertises",
                table: "Advertises",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.AdvertiseReports_dbo.Advertises_AdvertiseID",
                table: "AdvertiseReports",
                column: "AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Advertises_dbo.Advertises_Advertise_AdvertiseID",
                table: "Advertises",
                column: "Advertise_AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Advertises_Files_LicenseFileId",
                table: "Advertises",
                column: "LicenseFileId",
                principalTable: "Files",
                principalColumn: "FileID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Advertises_dbo.Files_PhotoID",
                table: "Advertises",
                column: "PhotoID",
                principalTable: "Files",
                principalColumn: "FileID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Advertises_dbo.Regions_Area",
                table: "Advertises",
                column: "Area",
                principalTable: "Regions",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Advertises_dbo.Regions_City",
                table: "Advertises",
                column: "City",
                principalTable: "Regions",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Advertises_dbo.Regions_Province",
                table: "Advertises",
                column: "Province",
                principalTable: "Regions",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Advertises_dbo.Users_UserID",
                table: "Advertises",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Carts_dbo.Advertises_AdvertiseID",
                table: "Carts",
                column: "AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.Comments_dbo.Advertises_AdvertiseID",
                table: "Comments",
                column: "AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.DiscountTables_dbo.Advertises_AdvertiseID",
                table: "DiscountTables",
                column: "AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.DynamicCategoryAdvertises_dbo.Advertises_Advertise_Id",
                table: "DynamicCategoryAdvertises",
                column: "Advertise_Id",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.ExtrinsicReserves_dbo.Advertises_AdvertiseID",
                table: "ExtrinsicReserves",
                column: "AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.FileAdvertises_dbo.Advertises_Advertise_Id",
                table: "FileAdvertises",
                column: "Advertise_Id",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstantReserveDate_Advertises_ResidenceId",
                table: "InstantReserveDate",
                column: "ResidenceId",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.OccupiedTables_dbo.Advertises_AdvertiseID",
                table: "OccupiedTables",
                column: "AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.PriceTables_dbo.Advertises_AdvertiseID",
                table: "PriceTables",
                column: "AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.ReportItems_dbo.Advertises_AdvertiseID",
                table: "ReportItems",
                column: "AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Advertises_ResidenceId",
                table: "Reserves",
                column: "ResidenceId",
                principalTable: "Advertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
