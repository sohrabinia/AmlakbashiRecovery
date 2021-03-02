using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class initialCore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Files",
            //    columns: table => new
            //    {
            //        FileID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        PostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        MinifyStatusInt = table.Column<int>(type: "int", nullable: false),
            //        MinifyMaxWidth = table.Column<int>(type: "int", nullable: false),
            //        MinifyQualityPercent = table.Column<long>(type: "bigint", nullable: false),
            //        MinifyStatus = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Files", x => x.FileID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "GroupPayments",
            //    columns: table => new
            //    {
            //        GroupPaymentID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TotalPrice = table.Column<long>(type: "bigint", nullable: false),
            //        PaidPrice = table.Column<long>(type: "bigint", nullable: false),
            //        ReserveIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        StatusInt = table.Column<int>(type: "int", nullable: false),
            //        CountPayments = table.Column<int>(type: "int", nullable: false),
            //        CountFailedPayments = table.Column<int>(type: "int", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        PayDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        PayListUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PayResultUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DownloadCount = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GroupPayments", x => x.GroupPaymentID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "InstantReserveAutoCancels",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        ReserveId = table.Column<long>(type: "bigint", nullable: false),
            //        SendSms = table.Column<bool>(type: "bit", nullable: false),
            //        Force = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_InstantReserveAutoCancels", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Posts",
            //    columns: table => new
            //    {
            //        PostID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FileID = table.Column<long>(type: "bigint", nullable: false),
            //        Abstract = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        PhotoID = table.Column<long>(type: "bigint", nullable: false),
            //        Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Posts", x => x.PostID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Regions",
            //    columns: table => new
            //    {
            //        RegionID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ParentID = table.Column<int>(type: "int", nullable: true),
            //        EnglishName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PersianName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Type = table.Column<int>(type: "int", nullable: false),
            //        Related = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CountAdvertise = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Regions", x => x.RegionID);
            //        table.ForeignKey(
            //            name: "FK_Regions_Regions_ParentID",
            //            column: x => x.ParentID,
            //            principalTable: "Regions",
            //            principalColumn: "RegionID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ReserveAutoCancels",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        ReserveId = table.Column<long>(type: "bigint", nullable: false),
            //        SendSms = table.Column<bool>(type: "bit", nullable: false),
            //        Force = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ReserveAutoCancels", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ReserveSendSms",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        initial = table.Column<bool>(type: "bit", nullable: false),
            //        userId = table.Column<int>(type: "int", nullable: false),
            //        type = table.Column<int>(type: "int", nullable: false),
            //        advertise_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        user_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        reserve_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        transaction_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        audience_mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        price = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        remain_price = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        doer_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        cause_string = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        code = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        extra_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        extra_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        extra_3 = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ReserveSendSms", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ServicePostItems",
            //    columns: table => new
            //    {
            //        ServicePostID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        PostID = table.Column<long>(type: "bigint", nullable: false),
            //        ServiceID = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServicePostItems", x => x.ServicePostID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Services",
            //    columns: table => new
            //    {
            //        ServiceID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ParentId = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Services", x => x.ServiceID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Settings",
            //    columns: table => new
            //    {
            //        SettingID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Settings", x => x.SettingID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        UserID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MainMobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LoginPriority = table.Column<int>(type: "int", nullable: false),
            //        FName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Tell = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ThirdPersonTell = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Mobile2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ForgetCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        AdminLoginCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        State = table.Column<int>(type: "int", nullable: false),
            //        Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        SendVerification = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ResponseFrom = table.Column<int>(type: "int", nullable: false),
            //        ResponseTo = table.Column<int>(type: "int", nullable: false),
            //        PhotoID = table.Column<long>(type: "bigint", nullable: true),
            //        ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OwnerShip = table.Column<int>(type: "int", nullable: false),
            //        AmlakbashiScore = table.Column<int>(type: "int", nullable: false),
            //        UserScore = table.Column<long>(type: "bigint", nullable: false),
            //        PhotoStatus = table.Column<int>(type: "int", nullable: false),
            //        Credit = table.Column<long>(type: "bigint", nullable: false),
            //        PrizeCredit = table.Column<long>(type: "bigint", nullable: false),
            //        UserGeneralType = table.Column<int>(type: "int", nullable: false),
            //        AccessType = table.Column<int>(type: "int", nullable: false),
            //        NotificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        AppNotificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FcmAppNotificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LastNotifPermitionTicks = table.Column<long>(type: "bigint", nullable: false),
            //        PresentorUserID = table.Column<int>(type: "int", nullable: false),
            //        PresentorPrizeGiven = table.Column<bool>(type: "bit", nullable: false),
            //        RecieveAppreciateDiscount = table.Column<bool>(type: "bit", nullable: false),
            //        CancelInstantReserveLimit = table.Column<int>(type: "int", nullable: false),
            //        InstantReserveAccess = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.UserID);
            //        table.ForeignKey(
            //            name: "FK_Users_Files_PhotoID",
            //            column: x => x.PhotoID,
            //            principalTable: "Files",
            //            principalColumn: "FileID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "DynamicCategories",
            //    columns: table => new
            //    {
            //        CategoryID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        City = table.Column<int>(type: "int", nullable: true),
            //        Province = table.Column<int>(type: "int", nullable: true),
            //        Area = table.Column<int>(type: "int", nullable: true),
            //        CountryDirection = table.Column<int>(type: "int", nullable: false),
            //        Type = table.Column<int>(type: "int", nullable: false),
            //        CountAdvertise = table.Column<int>(type: "int", nullable: false),
            //        CountView = table.Column<int>(type: "int", nullable: false),
            //        OldCountView = table.Column<int>(type: "int", nullable: false),
            //        AreaStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DescriptionH1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ShowDescription = table.Column<bool>(type: "bit", nullable: false),
            //        CustomUrlTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        RelatedCategoryIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        RegionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ParentRegionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TypeString = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CountAcc = table.Column<int>(type: "int", nullable: false),
            //        ParentCountAcc = table.Column<int>(type: "int", nullable: false),
            //        MinPrice = table.Column<long>(type: "bigint", nullable: false),
            //        MaxPrice = table.Column<long>(type: "bigint", nullable: false),
            //        ParentMinPrice = table.Column<int>(type: "int", nullable: false),
            //        ParentMaxPrice = table.Column<int>(type: "int", nullable: false),
            //        CityAreaListString = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CategoryPostID = table.Column<int>(type: "int", nullable: false),
            //        CategoryPostTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CategoryPostText = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        MostAccType = table.Column<int>(type: "int", nullable: false),
            //        ParentAccType = table.Column<int>(type: "int", nullable: false),
            //        RelatedItemsBehaviour = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DynamicCategories", x => x.CategoryID);
            //        table.ForeignKey(
            //            name: "FK_DynamicCategories_Regions_Area",
            //            column: x => x.Area,
            //            principalTable: "Regions",
            //            principalColumn: "RegionID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_DynamicCategories_Regions_City",
            //            column: x => x.City,
            //            principalTable: "Regions",
            //            principalColumn: "RegionID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_DynamicCategories_Regions_Province",
            //            column: x => x.Province,
            //            principalTable: "Regions",
            //            principalColumn: "RegionID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "actionLogs",
            //    columns: table => new
            //    {
            //        ActionLogID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Type = table.Column<int>(type: "int", nullable: false),
            //        RelatedID = table.Column<long>(type: "bigint", nullable: false),
            //        ActionSource = table.Column<int>(type: "int", nullable: false),
            //        PreviousData = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CurrentData = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_actionLogs", x => x.ActionLogID);
            //        table.ForeignKey(
            //            name: "FK_actionLogs_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Advertises",
            //    columns: table => new
            //    {
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Advertise_AdvertiseID = table.Column<long>(type: "bigint", nullable: true),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        PhotoID = table.Column<long>(type: "bigint", nullable: true),
            //        AlbumPhoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Province = table.Column<int>(type: "int", nullable: true),
            //        City = table.Column<int>(type: "int", nullable: true),
            //        Area = table.Column<int>(type: "int", nullable: true),
            //        CountryDirection = table.Column<int>(type: "int", nullable: false),
            //        WebVisit = table.Column<int>(type: "int", nullable: false),
            //        Overview = table.Column<int>(type: "int", nullable: false),
            //        ContactClick = table.Column<int>(type: "int", nullable: false),
            //        MetaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OldSlug = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        MetaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TypeID = table.Column<int>(type: "int", nullable: false),
            //        ParentAccType = table.Column<int>(type: "int", nullable: false),
            //        Region = table.Column<int>(type: "int", nullable: false),
            //        OwnershipType = table.Column<int>(type: "int", nullable: false),
            //        OwnerID = table.Column<int>(type: "int", nullable: false),
            //        OwnerMobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OwnerFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        NotVerifyReasons = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        AdvertiseScore = table.Column<long>(type: "bigint", nullable: false),
            //        AmlakbashiScore = table.Column<int>(type: "int", nullable: false),
            //        AdvertiseMode = table.Column<int>(type: "int", nullable: false),
            //        IsContactAvailable = table.Column<bool>(type: "bit", nullable: false),
            //        AllowParty = table.Column<bool>(type: "bit", nullable: false),
            //        AllowPets = table.Column<bool>(type: "bit", nullable: false),
            //        AllowSmoking = table.Column<bool>(type: "bit", nullable: false),
            //        EvidenceRequired = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OtherRules = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TodayIsEmpty = table.Column<bool>(type: "bit", nullable: false),
            //        Metrazh = table.Column<int>(type: "int", nullable: false),
            //        Elevator = table.Column<bool>(type: "bit", nullable: true),
            //        Parking = table.Column<int>(type: "int", nullable: false),
            //        Room = table.Column<int>(type: "int", nullable: false),
            //        Pool = table.Column<bool>(type: "bit", nullable: true),
            //        Capacity = table.Column<int>(type: "int", nullable: false),
            //        MoreThanCapacity = table.Column<int>(type: "int", nullable: false),
            //        DailyPrice = table.Column<int>(type: "int", nullable: false),
            //        NorouzPrice = table.Column<int>(type: "int", nullable: false),
            //        RentPrice = table.Column<long>(type: "bigint", nullable: false),
            //        HolidayPrice = table.Column<int>(type: "int", nullable: false),
            //        HolidayPikePrice = table.Column<int>(type: "int", nullable: false),
            //        PrepaymentPrice = table.Column<int>(type: "int", nullable: false),
            //        MoreThanCapacityPrice = table.Column<int>(type: "int", nullable: false),
            //        BuildingDirection = table.Column<int>(type: "int", nullable: false),
            //        LandArea = table.Column<int>(type: "int", nullable: false),
            //        Floor = table.Column<int>(type: "int", nullable: false),
            //        SingleBed = table.Column<int>(type: "int", nullable: false),
            //        DoublesBed = table.Column<int>(type: "int", nullable: false),
            //        Sauna = table.Column<bool>(type: "bit", nullable: true),
            //        Jacuzzi = table.Column<bool>(type: "bit", nullable: true),
            //        Bathroom = table.Column<bool>(type: "bit", nullable: true),
            //        Wifi = table.Column<bool>(type: "bit", nullable: true),
            //        WashingMachine = table.Column<bool>(type: "bit", nullable: true),
            //        MicrowaveOven = table.Column<bool>(type: "bit", nullable: true),
            //        SoundSystem = table.Column<bool>(type: "bit", nullable: true),
            //        Golf = table.Column<bool>(type: "bit", nullable: true),
            //        PoolTable = table.Column<bool>(type: "bit", nullable: true),
            //        Foosball = table.Column<bool>(type: "bit", nullable: true),
            //        Hairdryer = table.Column<bool>(type: "bit", nullable: true),
            //        TV = table.Column<bool>(type: "bit", nullable: true),
            //        Oven = table.Column<bool>(type: "bit", nullable: true),
            //        Refrigerator = table.Column<bool>(type: "bit", nullable: true),
            //        KitchenHood = table.Column<bool>(type: "bit", nullable: true),
            //        KitchenUtensils = table.Column<bool>(type: "bit", nullable: true),
            //        TeaMaker = table.Column<bool>(type: "bit", nullable: true),
            //        BlanketsAndMattresses = table.Column<int>(type: "int", nullable: false),
            //        ExtraBlanketCount = table.Column<int>(type: "int", nullable: false),
            //        HeatingSystem = table.Column<int>(type: "int", nullable: false),
            //        CoolingSystem = table.Column<int>(type: "int", nullable: false),
            //        WC = table.Column<int>(type: "int", nullable: false),
            //        Count = table.Column<int>(type: "int", nullable: false),
            //        Available = table.Column<bool>(type: "bit", nullable: false),
            //        HideInCategory = table.Column<bool>(type: "bit", nullable: false),
            //        AverageUserRating = table.Column<float>(type: "real", nullable: false),
            //        TidinessUserRating = table.Column<float>(type: "real", nullable: false),
            //        LocationString = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        BasePrice = table.Column<int>(type: "int", nullable: false),
            //        SupportInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        InstantReserveCancels = table.Column<int>(type: "int", nullable: false),
            //        InstantReserveStatus = table.Column<int>(type: "int", nullable: false),
            //        MaxInstantReserveStart = table.Column<int>(type: "int", nullable: false),
            //        MinReserveDays = table.Column<int>(type: "int", nullable: false),
            //        MaxReserveDays = table.Column<int>(type: "int", nullable: false),
            //        unixNorouzMinRequestDate = table.Column<long>(type: "bigint", nullable: false),
            //        NorouzOverCapacityPrice = table.Column<int>(type: "int", nullable: false),
            //        ImageThumbGenerateStatus = table.Column<int>(type: "int", nullable: false),
            //        Latitude = table.Column<double>(type: "float", nullable: false),
            //        Longitude = table.Column<double>(type: "float", nullable: false),
            //        HygieneProtocol = table.Column<bool>(type: "bit", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Advertises", x => x.AdvertiseID);
            //        table.ForeignKey(
            //            name: "FK_Advertises_Advertises_Advertise_AdvertiseID",
            //            column: x => x.Advertise_AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Advertises_Files_PhotoID",
            //            column: x => x.PhotoID,
            //            principalTable: "Files",
            //            principalColumn: "FileID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Advertises_Regions_Area",
            //            column: x => x.Area,
            //            principalTable: "Regions",
            //            principalColumn: "RegionID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Advertises_Regions_City",
            //            column: x => x.City,
            //            principalTable: "Regions",
            //            principalColumn: "RegionID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Advertises_Regions_Province",
            //            column: x => x.Province,
            //            principalTable: "Regions",
            //            principalColumn: "RegionID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Advertises_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "BankCards",
            //    columns: table => new
            //    {
            //        BankCardID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        FName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        BankCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ShabaNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        BankCardStatus = table.Column<int>(type: "int", nullable: false),
            //        ShabaStatus = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_BankCards", x => x.BankCardID);
            //        table.ForeignKey(
            //            name: "FK_BankCards_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "BlogPosts",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        LastModifyUserID = table.Column<int>(type: "int", nullable: false),
            //        PhotoID = table.Column<long>(type: "bigint", nullable: false),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        BlogLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        ShowingPlace = table.Column<int>(type: "int", nullable: false),
            //        Province = table.Column<int>(type: "int", nullable: false),
            //        City = table.Column<int>(type: "int", nullable: false),
            //        Area = table.Column<int>(type: "int", nullable: false),
            //        AccommodationType = table.Column<int>(type: "int", nullable: false),
            //        PositionType = table.Column<int>(type: "int", nullable: false),
            //        PoolStatus = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_BlogPosts", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_BlogPosts_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "DiscountCoupons",
            //    columns: table => new
            //    {
            //        ID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Type = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        PresentorUserID = table.Column<int>(type: "int", nullable: false),
            //        Percent = table.Column<int>(type: "int", nullable: false),
            //        UsingReserveID = table.Column<long>(type: "bigint", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DiscountCoupons", x => x.ID);
            //        table.ForeignKey(
            //            name: "FK_DiscountCoupons_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ReserveSupports",
            //    columns: table => new
            //    {
            //        ReserveSupportID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        SupporterID = table.Column<int>(type: "int", nullable: true),
            //        GuestID = table.Column<int>(type: "int", nullable: false),
            //        JourneyStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        StartSupportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LastSupporterActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ReservesWaitingForSupport = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ReservesSupporting = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ReservesSimilar = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TransferReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ReserveSupports", x => x.ReserveSupportID);
            //        table.ForeignKey(
            //            name: "FK_ReserveSupports_Users_GuestID",
            //            column: x => x.GuestID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ReserveSupports_Users_SupporterID",
            //            column: x => x.SupporterID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SupportChats",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: true),
            //        SupporterID = table.Column<int>(type: "int", nullable: true),
            //        CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastMessageTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SupportChats", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_SupportChats_Users_SupporterID",
            //            column: x => x.SupporterID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_SupportChats_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserFavorites",
            //    columns: table => new
            //    {
            //        FavoriteID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false),
            //        SetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        User_Id = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserFavorites", x => x.FavoriteID);
            //        table.ForeignKey(
            //            name: "FK_UserFavorites_Users_User_Id",
            //            column: x => x.User_Id,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AdvertiseReports",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false),
            //        Reason = table.Column<int>(type: "int", nullable: false),
            //        ReasonString = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AdvertiseReports", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AdvertiseReports_Advertises_AdvertiseID",
            //            column: x => x.AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Comments",
            //    columns: table => new
            //    {
            //        CommentID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false),
            //        PostID = table.Column<long>(type: "bigint", nullable: false),
            //        ParentID = table.Column<long>(type: "bigint", nullable: true),
            //        SenderUserID = table.Column<int>(type: "int", nullable: false),
            //        RecieverUserID = table.Column<int>(type: "int", nullable: true),
            //        type = table.Column<int>(type: "int", nullable: false),
            //        Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyDatetick = table.Column<long>(type: "bigint", nullable: false),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        SuspendReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        SeenByHost = table.Column<bool>(type: "bit", nullable: false),
            //        Score = table.Column<int>(type: "int", nullable: false),
            //        Likes = table.Column<int>(type: "int", nullable: false),
            //        Dislikes = table.Column<int>(type: "int", nullable: false),
            //        HostReplyId = table.Column<long>(type: "bigint", nullable: true),
            //        OperatorID = table.Column<int>(type: "int", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Comments", x => x.CommentID);
            //        table.ForeignKey(
            //            name: "FK_Comments_Advertises_AdvertiseID",
            //            column: x => x.AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Comments_Comments_HostReplyId",
            //            column: x => x.HostReplyId,
            //            principalTable: "Comments",
            //            principalColumn: "CommentID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Comments_Comments_ParentID",
            //            column: x => x.ParentID,
            //            principalTable: "Comments",
            //            principalColumn: "CommentID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Comments_Users_OperatorID",
            //            column: x => x.OperatorID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Comments_Users_RecieverUserID",
            //            column: x => x.RecieverUserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Comments_Users_SenderUserID",
            //            column: x => x.SenderUserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "DiscountTables",
            //    columns: table => new
            //    {
            //        DiscountTableID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false),
            //        From = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        To = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Percent = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DiscountTables", x => x.DiscountTableID);
            //        table.ForeignKey(
            //            name: "FK_DiscountTables_Advertises_AdvertiseID",
            //            column: x => x.AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "DynamicCategoryAdvertises",
            //    columns: table => new
            //    {
            //        Advertise_Id = table.Column<long>(type: "bigint", nullable: false),
            //        DynamicCategory_Id = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DynamicCategoryAdvertises", x => new { x.Advertise_Id, x.DynamicCategory_Id });
            //        table.ForeignKey(
            //            name: "FK_dbo.DynamicCategoryAdvertises_dbo.Advertises_Advertise_Id",
            //            column: x => x.Advertise_Id,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_dbo.DynamicCategoryAdvertises_dbo.DynamicCategories_DynamicCategory_Id",
            //            column: x => x.DynamicCategory_Id,
            //            principalTable: "DynamicCategories",
            //            principalColumn: "CategoryID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ExtrinsicReserves",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        NotifierUserID = table.Column<int>(type: "int", nullable: false),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false),
            //        HostUserID = table.Column<int>(type: "int", nullable: false),
            //        StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ExtrinsicReserves", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ExtrinsicReserves_Advertises_AdvertiseID",
            //            column: x => x.AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ExtrinsicReserves_Users_HostUserID",
            //            column: x => x.HostUserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ExtrinsicReserves_Users_NotifierUserID",
            //            column: x => x.NotifierUserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FileAdvertises",
            //    columns: table => new
            //    {
            //        Advertise_Id = table.Column<long>(type: "bigint", nullable: false),
            //        File_Id = table.Column<long>(type: "bigint", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FileAdvertises", x => new { x.Advertise_Id, x.File_Id });
            //        table.ForeignKey(
            //            name: "FK_dbo.FileAdvertises_dbo.Advertises_Advertise_Id",
            //            column: x => x.Advertise_Id,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_dbo.FileAdvertises_dbo.Files_File_Id",
            //            column: x => x.File_Id,
            //            principalTable: "Files",
            //            principalColumn: "FileID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PriceTables",
            //    columns: table => new
            //    {
            //        PriceTableID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false),
            //        Year = table.Column<int>(type: "int", nullable: false),
            //        Month = table.Column<int>(type: "int", nullable: false),
            //        Day = table.Column<int>(type: "int", nullable: false),
            //        Price = table.Column<int>(type: "int", nullable: false),
            //        UnixDate = table.Column<long>(type: "bigint", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PriceTables", x => x.PriceTableID);
            //        table.ForeignKey(
            //            name: "FK_PriceTables_Advertises_AdvertiseID",
            //            column: x => x.AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ReportItems",
            //    columns: table => new
            //    {
            //        ReportItemID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false),
            //        ReportID = table.Column<int>(type: "int", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LastModifyDatetick = table.Column<long>(type: "bigint", nullable: false),
            //        Score = table.Column<int>(type: "int", nullable: false),
            //        OperatorID = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ReportItems", x => x.ReportItemID);
            //        table.ForeignKey(
            //            name: "FK_ReportItems_Advertises_AdvertiseID",
            //            column: x => x.AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ReportItems_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Reserves",
            //    columns: table => new
            //    {
            //        ReserveID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        HostUserID = table.Column<int>(type: "int", nullable: false),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false),
            //        HostResponse = table.Column<int>(type: "int", nullable: false),
            //        StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        TotalPrice = table.Column<long>(type: "bigint", nullable: false),
            //        DepositPrice = table.Column<long>(type: "bigint", nullable: false),
            //        NumberOfGuests = table.Column<int>(type: "int", nullable: false),
            //        CancelState = table.Column<int>(type: "int", nullable: false),
            //        CancelDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CancelReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        HostResponseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        HostCallDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        GuestCallDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        PaymentGTAGRegistered = table.Column<bool>(type: "bit", nullable: false),
            //        SupportInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        SupportState = table.Column<int>(type: "int", nullable: false),
            //        SupporterIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        RatingShownToGuest = table.Column<bool>(type: "bit", nullable: false),
            //        shouldFollow = table.Column<bool>(type: "bit", nullable: false),
            //        GuestCallState = table.Column<int>(type: "int", nullable: false),
            //        HostCallState = table.Column<int>(type: "int", nullable: false),
            //        CancelDiscussion = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PaymentHasError = table.Column<bool>(type: "bit", nullable: false),
            //        ExcludeGroupPayment = table.Column<bool>(type: "bit", nullable: false),
            //        InstantReserve = table.Column<bool>(type: "bit", nullable: false),
            //        Archive = table.Column<bool>(type: "bit", nullable: false),
            //        InstantReserveCancelHost = table.Column<bool>(type: "bit", nullable: false),
            //        CouponID = table.Column<long>(type: "bigint", nullable: false),
            //        CouponPrice = table.Column<long>(type: "bigint", nullable: false),
            //        PrizePrice = table.Column<long>(type: "bigint", nullable: false),
            //        PrizeTransactionID = table.Column<long>(type: "bigint", nullable: false),
            //        CouponCalculationPrice = table.Column<long>(type: "bigint", nullable: false),
            //        DisableAutoCancel = table.Column<bool>(type: "bit", nullable: false),
            //        AccVisitedByGuest = table.Column<bool>(type: "bit", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Reserves", x => x.ReserveID);
            //        table.ForeignKey(
            //            name: "FK_Reserves_Advertises_AdvertiseID",
            //            column: x => x.AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Reserves_Users_HostUserID",
            //            column: x => x.HostUserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Reserves_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SupportChatMessages",
            //    columns: table => new
            //    {
            //        ID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SupportChatID = table.Column<long>(type: "bigint", nullable: false),
            //        UserID = table.Column<int>(type: "int", nullable: true),
            //        TypeInt = table.Column<int>(type: "int", nullable: false),
            //        ReadStatusInt = table.Column<int>(type: "int", nullable: false),
            //        Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //        Type = table.Column<int>(type: "int", nullable: false),
            //        ReadStatus = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SupportChatMessages", x => x.ID);
            //        table.ForeignKey(
            //            name: "FK_SupportChatMessages_SupportChats_SupportChatID",
            //            column: x => x.SupportChatID,
            //            principalTable: "SupportChats",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_SupportChatMessages_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Chats",
            //    columns: table => new
            //    {
            //        ChatID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ReserveID = table.Column<long>(type: "bigint", nullable: false),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        ChatStatus = table.Column<int>(type: "int", nullable: false),
            //        ReadStatus = table.Column<int>(type: "int", nullable: false),
            //        SupportReadStatus = table.Column<int>(type: "int", nullable: false),
            //        Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Chats", x => x.ChatID);
            //        table.ForeignKey(
            //            name: "FK_Chats_Reserves_ReserveID",
            //            column: x => x.ReserveID,
            //            principalTable: "Reserves",
            //            principalColumn: "ReserveID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Chats_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CreditTransactions",
            //    columns: table => new
            //    {
            //        CreditTransactionID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        Price = table.Column<long>(type: "bigint", nullable: false),
            //        Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        RemainedPrice = table.Column<long>(type: "bigint", nullable: false),
            //        BankTransactionID = table.Column<long>(type: "bigint", nullable: false),
            //        ReserveID = table.Column<long>(type: "bigint", nullable: true),
            //        TransactionCause = table.Column<int>(type: "int", nullable: false),
            //        TransactionCauseString = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        AdvertiseContactID = table.Column<long>(type: "bigint", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CreditTransactions", x => x.CreditTransactionID);
            //        table.ForeignKey(
            //            name: "FK_CreditTransactions_Reserves_ReserveID",
            //            column: x => x.ReserveID,
            //            principalTable: "Reserves",
            //            principalColumn: "ReserveID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_CreditTransactions_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "OccupiedTables",
            //    columns: table => new
            //    {
            //        OccupiedTableID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: false),
            //        ReserveID = table.Column<long>(type: "bigint", nullable: true),
            //        ExtrinsicReserveID = table.Column<long>(type: "bigint", nullable: true),
            //        Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OccupiedTables", x => x.OccupiedTableID);
            //        table.ForeignKey(
            //            name: "FK_OccupiedTables_Advertises_AdvertiseID",
            //            column: x => x.AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_OccupiedTables_ExtrinsicReserves_ExtrinsicReserveID",
            //            column: x => x.ExtrinsicReserveID,
            //            principalTable: "ExtrinsicReserves",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_OccupiedTables_Reserves_ReserveID",
            //            column: x => x.ReserveID,
            //            principalTable: "Reserves",
            //            principalColumn: "ReserveID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Payments",
            //    columns: table => new
            //    {
            //        PaymentID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Authority = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        RefID = table.Column<long>(type: "bigint", nullable: false),
            //        TotalPrice = table.Column<long>(type: "bigint", nullable: false),
            //        Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        BankId = table.Column<int>(type: "int", nullable: false),
            //        ReserveID = table.Column<long>(type: "bigint", nullable: true),
            //        CouponID = table.Column<long>(type: "bigint", nullable: false),
            //        PrizePrice = table.Column<long>(type: "bigint", nullable: false),
            //        ReservePrice = table.Column<long>(type: "bigint", nullable: false),
            //        ProductType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        PayDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Payments", x => x.PaymentID);
            //        table.ForeignKey(
            //            name: "FK_Payments_Reserves_ReserveID",
            //            column: x => x.ReserveID,
            //            principalTable: "Reserves",
            //            principalColumn: "ReserveID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Payments_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PrizeCreditTransactions",
            //    columns: table => new
            //    {
            //        ID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        Price = table.Column<long>(type: "bigint", nullable: false),
            //        Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        RemainedPrice = table.Column<long>(type: "bigint", nullable: false),
            //        Type = table.Column<int>(type: "int", nullable: false),
            //        ReserveID = table.Column<long>(type: "bigint", nullable: true),
            //        CustomTitle = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PrizeCreditTransactions", x => x.ID);
            //        table.ForeignKey(
            //            name: "FK_PrizeCreditTransactions_Reserves_ReserveID",
            //            column: x => x.ReserveID,
            //            principalTable: "Reserves",
            //            principalColumn: "ReserveID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_PrizeCreditTransactions_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ReservePayments",
            //    columns: table => new
            //    {
            //        ReservePaymentID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        ReserveID = table.Column<long>(type: "bigint", nullable: false),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        OperatorID = table.Column<int>(type: "int", nullable: false),
            //        TransactionID = table.Column<long>(type: "bigint", nullable: false),
            //        RefID = table.Column<long>(type: "bigint", nullable: false),
            //        PaymentType = table.Column<int>(type: "int", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Price = table.Column<long>(type: "bigint", nullable: false),
            //        PaymentMethod = table.Column<int>(type: "int", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ReservePayments", x => x.ReservePaymentID);
            //        table.ForeignKey(
            //            name: "FK_ReservePayments_Reserves_ReserveID",
            //            column: x => x.ReserveID,
            //            principalTable: "Reserves",
            //            principalColumn: "ReserveID",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ReservePayments_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Carts",
            //    columns: table => new
            //    {
            //        CartID = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        AmlakID = table.Column<int>(type: "int", nullable: false),
            //        AdvertiseID = table.Column<long>(type: "bigint", nullable: true),
            //        ReserveID = table.Column<long>(type: "bigint", nullable: true),
            //        BannerID = table.Column<int>(type: "int", nullable: false),
            //        UserID = table.Column<int>(type: "int", nullable: false),
            //        Count = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        Price = table.Column<long>(type: "bigint", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        PayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Payment_PaymentID = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Carts", x => x.CartID);
            //        table.ForeignKey(
            //            name: "FK_Carts_Advertises_AdvertiseID",
            //            column: x => x.AdvertiseID,
            //            principalTable: "Advertises",
            //            principalColumn: "AdvertiseID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Carts_Payments_Payment_PaymentID",
            //            column: x => x.Payment_PaymentID,
            //            principalTable: "Payments",
            //            principalColumn: "PaymentID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Carts_Reserves_ReserveID",
            //            column: x => x.ReserveID,
            //            principalTable: "Reserves",
            //            principalColumn: "ReserveID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Carts_Users_UserID",
            //            column: x => x.UserID,
            //            principalTable: "Users",
            //            principalColumn: "UserID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_actionLogs_UserID",
            //    table: "actionLogs",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AdvertiseReports_AdvertiseID",
            //    table: "AdvertiseReports",
            //    column: "AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Advertises_Advertise_AdvertiseID",
            //    table: "Advertises",
            //    column: "Advertise_AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Advertises_Area",
            //    table: "Advertises",
            //    column: "Area");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Advertises_City",
            //    table: "Advertises",
            //    column: "City");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Advertises_PhotoID",
            //    table: "Advertises",
            //    column: "PhotoID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Advertises_Province",
            //    table: "Advertises",
            //    column: "Province");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Advertises_UserID",
            //    table: "Advertises",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_BankCards_UserID",
            //    table: "BankCards",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_BlogPosts_UserID",
            //    table: "BlogPosts",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Carts_AdvertiseID",
            //    table: "Carts",
            //    column: "AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Carts_Payment_PaymentID",
            //    table: "Carts",
            //    column: "Payment_PaymentID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Carts_ReserveID",
            //    table: "Carts",
            //    column: "ReserveID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Carts_UserID",
            //    table: "Carts",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Chats_ReserveID",
            //    table: "Chats",
            //    column: "ReserveID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Chats_UserID",
            //    table: "Chats",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_AdvertiseID",
            //    table: "Comments",
            //    column: "AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_HostReplyId",
            //    table: "Comments",
            //    column: "HostReplyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_OperatorID",
            //    table: "Comments",
            //    column: "OperatorID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_ParentID",
            //    table: "Comments",
            //    column: "ParentID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_RecieverUserID",
            //    table: "Comments",
            //    column: "RecieverUserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_SenderUserID",
            //    table: "Comments",
            //    column: "SenderUserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CreditTransactions_ReserveID",
            //    table: "CreditTransactions",
            //    column: "ReserveID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CreditTransactions_UserID",
            //    table: "CreditTransactions",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_DiscountCoupons_UserID",
            //    table: "DiscountCoupons",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_DiscountTables_AdvertiseID",
            //    table: "DiscountTables",
            //    column: "AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_DynamicCategories_Area",
            //    table: "DynamicCategories",
            //    column: "Area");

            //migrationBuilder.CreateIndex(
            //    name: "IX_DynamicCategories_City",
            //    table: "DynamicCategories",
            //    column: "City");

            //migrationBuilder.CreateIndex(
            //    name: "IX_DynamicCategories_Province",
            //    table: "DynamicCategories",
            //    column: "Province");

            //migrationBuilder.CreateIndex(
            //    name: "IX_DynamicCategoryAdvertises_DynamicCategory_Id",
            //    table: "DynamicCategoryAdvertises",
            //    column: "DynamicCategory_Id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExtrinsicReserves_AdvertiseID",
            //    table: "ExtrinsicReserves",
            //    column: "AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExtrinsicReserves_HostUserID",
            //    table: "ExtrinsicReserves",
            //    column: "HostUserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExtrinsicReserves_NotifierUserID",
            //    table: "ExtrinsicReserves",
            //    column: "NotifierUserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_FileAdvertises_File_Id",
            //    table: "FileAdvertises",
            //    column: "File_Id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OccupiedTables_AdvertiseID",
            //    table: "OccupiedTables",
            //    column: "AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OccupiedTables_ExtrinsicReserveID",
            //    table: "OccupiedTables",
            //    column: "ExtrinsicReserveID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OccupiedTables_ReserveID",
            //    table: "OccupiedTables",
            //    column: "ReserveID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Payments_ReserveID",
            //    table: "Payments",
            //    column: "ReserveID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Payments_UserID",
            //    table: "Payments",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PriceTables_AdvertiseID",
            //    table: "PriceTables",
            //    column: "AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PrizeCreditTransactions_ReserveID",
            //    table: "PrizeCreditTransactions",
            //    column: "ReserveID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PrizeCreditTransactions_UserID",
            //    table: "PrizeCreditTransactions",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Regions_ParentID",
            //    table: "Regions",
            //    column: "ParentID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ReportItems_AdvertiseID",
            //    table: "ReportItems",
            //    column: "AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ReportItems_UserID",
            //    table: "ReportItems",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ReservePayments_ReserveID",
            //    table: "ReservePayments",
            //    column: "ReserveID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ReservePayments_UserID",
            //    table: "ReservePayments",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Reserves_AdvertiseID",
            //    table: "Reserves",
            //    column: "AdvertiseID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Reserves_HostUserID",
            //    table: "Reserves",
            //    column: "HostUserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Reserves_UserID",
            //    table: "Reserves",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ReserveSupports_GuestID",
            //    table: "ReserveSupports",
            //    column: "GuestID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ReserveSupports_SupporterID",
            //    table: "ReserveSupports",
            //    column: "SupporterID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SupportChatMessages_SupportChatID",
            //    table: "SupportChatMessages",
            //    column: "SupportChatID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SupportChatMessages_UserID",
            //    table: "SupportChatMessages",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SupportChats_SupporterID",
            //    table: "SupportChats",
            //    column: "SupporterID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SupportChats_UserID",
            //    table: "SupportChats",
            //    column: "UserID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserFavorites_User_Id",
            //    table: "UserFavorites",
            //    column: "User_Id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_PhotoID",
            //    table: "Users",
            //    column: "PhotoID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "actionLogs");

            //migrationBuilder.DropTable(
            //    name: "AdvertiseReports");

            //migrationBuilder.DropTable(
            //    name: "BankCards");

            //migrationBuilder.DropTable(
            //    name: "BlogPosts");

            //migrationBuilder.DropTable(
            //    name: "Carts");

            //migrationBuilder.DropTable(
            //    name: "Chats");

            //migrationBuilder.DropTable(
            //    name: "Comments");

            //migrationBuilder.DropTable(
            //    name: "CreditTransactions");

            //migrationBuilder.DropTable(
            //    name: "DiscountCoupons");

            //migrationBuilder.DropTable(
            //    name: "DiscountTables");

            //migrationBuilder.DropTable(
            //    name: "DynamicCategoryAdvertises");

            //migrationBuilder.DropTable(
            //    name: "FileAdvertises");

            //migrationBuilder.DropTable(
            //    name: "GroupPayments");

            //migrationBuilder.DropTable(
            //    name: "InstantReserveAutoCancels");

            //migrationBuilder.DropTable(
            //    name: "OccupiedTables");

            //migrationBuilder.DropTable(
            //    name: "Posts");

            //migrationBuilder.DropTable(
            //    name: "PriceTables");

            //migrationBuilder.DropTable(
            //    name: "PrizeCreditTransactions");

            //migrationBuilder.DropTable(
            //    name: "ReportItems");

            //migrationBuilder.DropTable(
            //    name: "ReserveAutoCancels");

            //migrationBuilder.DropTable(
            //    name: "ReservePayments");

            //migrationBuilder.DropTable(
            //    name: "ReserveSendSms");

            //migrationBuilder.DropTable(
            //    name: "ReserveSupports");

            //migrationBuilder.DropTable(
            //    name: "ServicePostItems");

            //migrationBuilder.DropTable(
            //    name: "Services");

            //migrationBuilder.DropTable(
            //    name: "Settings");

            //migrationBuilder.DropTable(
            //    name: "SupportChatMessages");

            //migrationBuilder.DropTable(
            //    name: "UserFavorites");

            //migrationBuilder.DropTable(
            //    name: "Payments");

            //migrationBuilder.DropTable(
            //    name: "DynamicCategories");

            //migrationBuilder.DropTable(
            //    name: "ExtrinsicReserves");

            //migrationBuilder.DropTable(
            //    name: "SupportChats");

            //migrationBuilder.DropTable(
            //    name: "Reserves");

            //migrationBuilder.DropTable(
            //    name: "Advertises");

            //migrationBuilder.DropTable(
            //    name: "Regions");

            //migrationBuilder.DropTable(
            //    name: "Users");

            //migrationBuilder.DropTable(
            //    name: "Files");
        }
    }
}
