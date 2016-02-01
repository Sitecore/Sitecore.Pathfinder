// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data;

namespace Sitecore
{
    public static class ArchivedItemFieldIDs
    {
        public static readonly ID ArchiveDate = new ID("{38E9A00A-933B-4A9A-9D82-E2171CA52756}");

        public static readonly ID Completed = new ID("{A42AD672-680A-47C0-899D-04238C8F15BC}");

        public static readonly ID DatabaseName = new ID("{1897F98D-6DCD-4083-B1D3-BC9C4B11DD27}");

        public static readonly ID ItemID = new ID("{2F093D15-FD21-47A0-ACC0-E153B9BA508C}");

        public static readonly ID ItemXml = new ID("{67A5FC75-1996-44E2-B226-A7F92AC9F9C7}");

        public static readonly ID ParentID = new ID("{1F4412CC-609C-4D3C-AF8C-D5C849202916}");

        public static readonly ID UserName = new ID("{FE3A1E84-098D-41FE-9652-3F7821A8889A}");
    }

    public static class CommandFieldIDs
    {
        public static readonly ID Method = new ID("{4BC75539-D5C4-487B-AF35-FF13D62BD286}");

        public static readonly ID Type = new ID("{752579A7-EF2F-45B7-B9D1-9B682308B7A5}");
    }

    public static class CommandsCommandFieldIDs
    {
        public static readonly ID Type = new ID("{183D65C7-DF37-465C-81CD-D9BC6B143D0D}");
    }

    public static class DeviceFieldIDs
    {
        public static readonly ID Agent = new ID("{E2E60CB8-AFAE-4465-8443-40D3A95EEF0F}");

        public static readonly ID Default = new ID("{3A5475C5-D476-45F2-933F-67B73B1888A2}");

        public static readonly ID DefaultSimulator = new ID("{45C2550C-E38E-48FA-A1D7-484F390F126C}");

        public static readonly ID FallbackDevice = new ID("{160B90C2-4011-4E89-9C5E-C617DDFBC8C7}");

        public static readonly ID QueryString = new ID("{32326F99-C0DA-4817-9DBE-79E6DE8005E4}");
    }

    public static class FieldIDs
    {
        #region Constants

        public static readonly ID ArchiveDate = new ID("{56C15C6D-FD5A-40CA-BB37-64CEEC6A9BD5}");

        public static readonly ID ArchiveVersionDate = new ID("{1D99005E-65CA-45CA-9D9A-FD7016E23F1E}");

        public static readonly ID BaseTemplate = new ID("{12C33F3F-86C5-43A5-AEB4-5598CEC45116}");

        public static readonly ID SubitemsSorting = new ID("{6FD695E7-7F6D-4CA5-8B49-A829E5950AE9}");

        public static readonly ID Command = new ID("{854CC8F6-94AD-4521-A4B6-44ED8F794C98}");

        public static readonly ID CommentDialogHeight = new ID("{9D3D61EF-6F65-4F71-ABD4-9172549B15C2}");

        public static readonly ID CommentTemplate = new ID("{F1090046-2F13-4F19-BB41-C75DF4F89516}");

        public static readonly ID ContextMenu = new ID("{D3AE7222-425D-4B77-95D8-EE33AC2B6730}");

        public static readonly ID Created = new ID("{25BED78C-4957-4165-998A-CA1B52F67497}");

        public static readonly ID CreatedBy = new ID("{5DD74568-4D4B-44C1-B513-0AF5F4CDA34F}");

        public static readonly ID DefaultDomain = new ID("{35374AF0-8C35-44C9-8580-783A9CD511E7}");

        public static readonly ID DefaultWorkflow = new ID("{CA9B9F52-4FB0-4F87-A79F-24DEA62CDA65}");

        public static readonly ID DefaultCommentDialogHeight = new ID("{591F16DA-4020-4FFD-9560-EF4996FC2A61}");

        public static readonly ID DefaultCommentTemplate = new ID("{2A59C3DC-E9AB-4108-BEB2-1CA9AEE56A33}");

        public static readonly ID DictionaryKey = new ID("{580C75A8-C01A-4580-83CB-987776CEB3AF}");

        public static readonly ID DictionaryPhrase = new ID("{2BA3454A-9A9C-4CDF-A9F8-107FD484EB6E}");

        public static readonly ID DisplayName = new ID("{B5E02AD9-D56F-4C41-A065-A133DB87BDEB}");

        public static readonly ID DomainRoleNameTemplate = new ID("{14CAC558-13DE-4795-A999-50A6B74D88BD}");

        public static readonly ID DomainUserNameTemplate = new ID("{B68F6FCE-A6BC-4A8D-930D-407F6F72E79C}");

        public static readonly ID DomainMembershipProvider = new ID("{E6509F86-520E-4F08-8D16-D9CB9A81E2FE}");

        public static readonly ID DomainUniqueName = new ID("{FF62DBCC-FEA8-4373-8014-09E97362911B}");

        public static readonly ID EditorPath = new ID("{EED015E2-914C-469E-8B12-0B86EE18E5CF}");

        public static readonly ID FallbackDomain = new ID("{CD72E892-D8BB-487C-9EB1-6A57C48C72CC}");

        public static readonly ID Hidden = new ID("{39C4902E-9960-4469-AEEF-E878E9C8218F}");

        public static readonly ID HideVersion = new ID("{B8F42732-9CB8-478D-AE95-07E25345FB0F}");

        public static readonly ID Icon = new ID("{06D5295C-ED2F-4A54-9BF2-26228D113318}");

        public static readonly ID InheritSecurity = new ID("{6917D2D9-3F44-49FB-9319-0EB7D900D8DC}");

        public static readonly ID LanguageIso = new ID("{C437E416-8948-427D-A982-8ED37AE3F553}");

        [Obsolete("This field is no longer used - please use LayoutField instead.")]
        public static readonly ID Layout = new ID("{E1D68787-D22B-4EA2-82B3-84C282E375EB}");

        public static readonly ID Lock = new ID("{001DD393-96C5-490B-924A-B0F25CD9EFD8}");

        public static readonly ID Branches = new ID("{1172F251-DAD4-4EFB-A329-0C63500E4F1E}");

        [Obsolete("Deprecated - Use Branches instead.")]
        public static readonly ID Masters = new ID("{1172F251-DAD4-4EFB-A329-0C63500E4F1E}");

        public static readonly ID NextState = new ID("{DCBEBC58-6124-4100-A248-FC717D6C78D5}");

        public static readonly ID NeverPublish = new ID("{9135200A-5626-4DD8-AB9D-D665B8C11748}");

        public static readonly ID Originator = new ID("{F6D8A61C-2F84-4401-BD24-52D2068172BC}");

        public static readonly ID PageDefinition = new ID("{8FFF5707-C288-4CDF-B3CE-4851EAD9C16C}");

        public static readonly ID Presentation = new ID("{978EFCEB-A3B1-430F-9BB6-B8A93D436932}");

        //public static readonly ID ProxyExcludeChildren = new ID("{663EDEB2-9C25-4A76-BF47-28BA531F7EA8}");

        public static readonly ID Preview = new ID("{41C6CC0E-389F-4D51-9990-FE35417B6666}");

        public static readonly ID ProxyInsertionType = new ID("{E4A55896-CF58-4C9C-8284-F01B141B0E85}");

        public static readonly ID ProxyTargetItem = new ID("{98B1EE13-D363-49FE-93C4-F48DC1191008}");

        public static readonly ID ProxySourceDatabase = new ID("{02BD4E57-EBAC-4C04-935A-8A6B6D551C5C}");

        public static readonly ID ProxySourceItem = new ID("{C5211535-E9A7-4214-84BC-93F240A23602}");

        public static readonly ID PublishDate = new ID("{86FE4F77-4D9A-4EC3-9ED9-263D03BD1965}");

        public static readonly ID PublishingTargets = new ID("{74484BDF-7C86-463C-B49F-7B73B9AFC965}");

        public static readonly ID PublishingTargetDatabase = new ID("{39ECFD90-55D2-49D8-B513-99D15573DE41}");

        public static readonly ID ReminderDate = new ID("{ABE5D54C-59D7-41E6-8D3F-C1A3E4EC9B9E}");

        public static readonly ID ReminderRecipients = new ID("{2ED9C4D0-9EFF-490D-A40A-B5D856499C40}");

        public static readonly ID ReminderText = new ID("{BB6C8540-118E-4C49-9157-830576D7345A}");

        public static readonly ID Renderers = new ID("{B03569B1-1534-43F2-8C83-BD064B7D782C}");

        public static readonly ID Ribbon = new ID("{0C894AAB-962B-4A84-B923-CB24B05E60D2}");

        public static readonly ID LayoutField = new ID("{F1A1FE9E-A60C-4DDB-A3A0-BB5B29FE732E}");

        public static readonly ID ReadOnly = new ID("{9C6106EA-7A5A-48E2-8CAD-F0F693B1E2D4}");

        public static readonly ID Reference = new ID("{C9CE97FB-3E8E-4384-9430-7DBA75B275DD}");

        public static readonly ID Revision = new ID("{8CDC337E-A112-42FB-BBB4-4143751E123F}");

        public static readonly ID Owner = new ID("{52807595-0F8F-4B20-8D2A-CB71D28C6103}");

        public static readonly ID Security = new ID("{DEC8D2D5-E3CF-48B6-A653-8E69E2716641}");

        public static readonly ID Skin = new ID("{079AFCFE-8ACA-4863-BDA7-07893541E2F5}");

        public static readonly ID Sortorder = new ID("{BA3F86A2-4A1C-4D78-B63D-91C2779C1B5E}");

        public static readonly ID StandardValues = new ID("{F7D48A55-2158-4F02-9356-756654404F73}");

        public static readonly ID StandardValueHolderId = new ID("{716D0C7F-84F9-4fc8-9817-E489A8DC07AE}");

        public static readonly ID State = new ID("{3E431DE1-525E-47A3-B6B0-1CCBEC3A8C98}");

        public static readonly ID AppearanceEvaluatorType = new ID("{DF36505E-A70A-4414-A9AE-AE26DB86FE19}");

        public static readonly ID Style = new ID("{A791F095-2521-4B4D-BEF9-21DDA221F608}");

        public static readonly ID Thumbnail = new ID("{C7C26117-DBB1-42B2-AB5E-F7223845CCA3}");

        public static readonly ID UnpublishDate = new ID("{7EAD6FD6-6CF1-4ACA-AC6B-B200E7BAFE88}");

        public static readonly ID Updated = new ID("{D9CF14B1-FA16-4BA6-9288-E8A174D4D522}");

        public static readonly ID UpdatedBy = new ID("{BADD9CF9-53E0-4D0C-BCC0-2D784C282F6A}");

        public static readonly ID UserMembership = new ID("{1F37F0DD-4E61-4C90-984D-8B24317D6B32}");

        public static readonly ID ValidFrom = new ID("{C8F93AFE-BFD4-4E8F-9C61-152559854661}");

        public static readonly ID ValidTo = new ID("{4C346442-E859-4EFD-89B2-44AEDF467D21}");

        public static readonly ID FinalLayoutField = new ID("{04BF00DB-F5FB-41F7-8AB7-22408372A981}");

        public static readonly ID Workflow = new ID("{A4F985D9-98B3-4B52-AAAF-4344F6E747C6}");

        public static readonly ID WorkflowState = new ID("{3E431DE1-525E-47A3-B6B0-1CCBEC3A8C98}");

        public static readonly ID Source = new ID("{1B86697D-60CA-4D80-83FB-7555A2E6CE1C}");

        public static readonly ID UIStaticItem = new ID("{C67177BF-D4F5-41B3-BE72-7D9AFF76F41E}");

        public static readonly ID StandardFieldsID = new ID("{C29C1D32-9FB7-4DA5-8508-3FEC2AF3A81D}");

        #endregion
    }

    public static class FieldButtonIDs
    {
        #region Constants

        public static ID AssignSecurity = new ID("{FAACBB67-1480-48FA-B401-2C5D1FE6F3AB}");

        #endregion
    }

    public static class ItemIDs
    {
        #region Constants

        public static readonly ID AnonymousUser = new ID("{4AF789F7-750F-45C1-B4F0-669A6348482E}");

        public static readonly ID ConditionalRenderingsGlobalRules = new ID("{6892B190-D0C8-4628-A179-24D197AB0C07}");

        public static readonly ID ContentRoot = new ID("{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}");

        public static readonly ID DefaultRibbon = new ID("{073BBB5D-65B5-485F-A1F8-64E55C84696E}");

        public static readonly ID DevicesRoot = new ID("{E18F4BC6-46A2-4842-898B-B6613733F06F}");

        public static readonly ID Dictionary = new ID("{504AE189-9F36-4C62-9767-66D73D6C3084}");

        public static readonly ID EveryoneRoleID = new ID("{00088163-665D-4F6F-9E63-C0CF1FB4E2FE}");

        public static readonly ID LanguageRoot = new ID("{64C4F646-A3FA-4205-B98E-4DE2C609B60F}");

        public static readonly ID LayoutRoot = new ID("{EB2E4FFD-2761-4653-B052-26A64D385227}");

        public static readonly ID Layouts = new ID("{75CC5CE4-8979-4008-9D3C-806477D57619}");

        public static readonly ID BranchesRoot = new ID("{BAD98E0E-C1B5-4598-AC13-21B06218B30C}");

        [Obsolete("Deprecated - Use BranchesRoot instead.")]
        public static readonly ID MastersRoot = new ID("{BAD98E0E-C1B5-4598-AC13-21B06218B30C}");

        public static readonly ID MediaLibraryRoot = new ID("{3D6658D8-A0BF-4E75-B3E2-D050FABCF4E1}");

        public static readonly ID Null = new ID("{00000000-0000-0000-0000-000000000000}");

        public static readonly ID PlaceholderSettingsRoot = new ID("{1CE3B36C-9B0C-4EB5-A996-BFCB4EAA5287}");

        public static readonly ID RootID = new ID("{11111111-1111-1111-1111-111111111111}");

        public static readonly ID Policies = new ID("{1E7C8D5A-51CF-42A7-8D58-0752B3E39C8B}");

        public static readonly ID TemplateRoot = new ID("{3C1715FE-6A13-4FCF-845F-DE308BA9741D}");

        public static readonly ID WorkflowRoot = new ID("{05592656-56D7-4D85-AACF-30919EE494F9}");

        public static readonly ID Shell = new ID("{4616E2BE-BF68-4D22-91B3-93301C9F86B7}");

        public static readonly ID ShellAll = new ID("{DF4F23E3-9BAC-42D6-A249-E50CA7475FFD}");

        public static readonly ID ShellDefault = new ID("{A8653DDD-862E-418F-A312-BD543157E354}");

        public static readonly ID SystemRoot = new ID("{13D6D6C6-C50B-4BBD-B331-2B04F1A58F21}");

        [Obsolete("This ID has been deprecated.")]
        public static readonly ID VirtualStructures = new ID("{6542A6DC-2859-4041-8C3E-E356BF390DC9}");

        public static readonly ID Undefined = new ID("{FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF}");

        public static class Analytics
        {
            public static readonly ID Criteria = new ID("{8463C2CD-2829-4B44-80C8-0B09B391D696}");

            public static readonly ID DashboardReportsItem = new ID("{3A81F528-EAC1-4B33-90C0-B261584855DD}");

            public static readonly ID DefaultCondition = new ID("{00000000-0000-0000-0000-000000000000}");

            public static readonly ID GlobalReports = new ID("{63AE78FF-7DAA-4B80-9A93-0D8145269716}");

            public static readonly ID Macros = new ID("{1B4FAD92-BCB4-4842-A8F0-23215634A0E4}");

            public static readonly ID MarketingCenterItem = new ID("{33CFB9CA-F565-4D5B-B88A-7CDFE29A6D71}");

            public static readonly ID OrganicBrandedKeywords = new ID("{B9985DD1-D81A-43BB-B7F7-C1294FEC09F4}");

            public static readonly ID PageEvents = new ID("{633273C1-02A5-4EBC-9B82-BD1A7C684FEA}");

            public static readonly ID Profiles = new ID("{12BD7E35-437B-449C-B931-23CFA12C03D8}");

            public static readonly ID TrafficTypesItem = new ID("{7E265978-8C1B-419D-BC06-0B5D101F04DF}");

            public static readonly ID VisitorIdentifications = new ID("{C0BE89B5-4061-4276-9C4F-569EDC4EEF06}");

            public static readonly ID VisitorIdentificationTypes = new ID("{220E8575-DA98-4F87-97A6-34940BEA0109}");

            public static class MarketingCenter
            {
                public static readonly ID Campaigns = new ID("{EC095310-746F-4C1B-A73F-941863564DC2}");

                public static readonly ID Goals = new ID("{0CB97A9F-CAFB-42A0-8BE1-89AB9AE32BD9}");

                public static readonly ID Policies = new ID("{DB40C9D3-5DB3-4831-A0A9-53AB17EED652}");

                public static readonly ID ReportFilters = new ID("{5B3FE22D-1DC9-4CC4-9665-1A3F2DDD6732}");

                public static readonly ID TestLaboratory = new ID("{BA1B87AC-0853-45F0-AE13-41F969540134}");
            }

            public static class Reports
            {
                public static readonly ID ItemReports = new ID("{B62E6C0C-8BE3-4F51-939A-DB039EEFA3A4}");
            }
        }

        #endregion
    }

    public static class RibbonButtonIDs
    {
        #region Constants

        public static readonly ID NewButtonID = new ID("{A041AD04-2A50-4737-80FA-F6C80A9DB2DB}");

        #endregion
    }

    public static class LayoutFieldIDs
    {
        public static readonly ID Path = new ID("{A036B2BC-BA04-44F6-A75F-BAE6CD242ABF}");
    }

    public static class PackageRegistrationFieldIDs
    {
        public static readonly ID PackageID = new ID("{AAFA3255-E19D-4C2B-A4A3-9C90D0623CDF}");

        public static readonly ID PackageName = new ID("{D79648F3-1351-4A3C-822E-86809DB16479}");

        public static readonly ID PackageVersion = new ID("{5322A457-A605-4A38-A0D1-F944B7F781D9}");
    }

    public static class PublishingTargetFieldIDs
    {
        public static readonly ID PreviewPublishingTarget = new ID("{17394C5A-35A3-45A3-BF2C-F1AC607D1476}");
    }

    public static class ScheduleFieldIDs
    {
        public static readonly ID Async = new ID("{A2C0C7A2-697D-40E2-9516-54E0C2D6028E}");

        public static readonly ID AutoRemove = new ID("{470A5C23-5361-4260-96EB-3668E5933303}");

        public static readonly ID Command = new ID("{62E64DBB-0A39-4DD7-BA05-7BD08C5404E0}");

        public static readonly ID Items = new ID("{70893EF5-98E6-4721-844A-C364D0D9E48E}");

        public static readonly ID LastRun = new ID("{B1E16562-F3F9-4DDD-84CA-6E099950ECC0}");

        public static readonly ID Schedule = new ID("{50BE51F8-746D-4DC2-9C3B-B14ABC5CE9B7}");
    }

    public static class ShellFieldIDs
    {
        #region Constants

        public static readonly ID Commands = new ID("{254D83B4-F33E-484C-8C19-857423641935}");

        public static readonly ID Deep = new ID("{713F5944-AC19-4F0E-8EAE-EB2881D64DCD}");

        public static readonly ID FolderKind = new ID("{F47FD34D-279B-41E8-8638-0DF52AA9C6AC}");

        public static readonly ID Branch = new ID("{24E43EF3-A36D-4294-B8A5-CDF03F0446ED}");

        [Obsolete("Deprecated - Use Branch instead.")]
        public static readonly ID Master = new ID("{24E43EF3-A36D-4294-B8A5-CDF03F0446ED}");

        public static readonly ID Path = new ID("{AC39F7B7-D0C8-4875-B1A6-4D25102BE816}");

        public static readonly ID Template = new ID("{FB579D68-AE9A-4A7D-B36A-CA87D60E6E1C}");

        public static readonly ID ToolbarIcon = new ID("{87751373-016B-4902-B0A8-6148D9F782A5}");

        public static readonly ID ToolbarStylesheetClass = new ID("{E3A2EE9E-730A-40C6-A5C0-4BB08E9BE981}");

        #endregion
    }

    [Obsolete("Deprecated.")]
    public static class SystemFieldIDs
    {
        #region Constants

        public static readonly Guid MasterId = new Guid("{10FFF49D-AAB7-40C1-AC76-A38A68609864}");

        public static readonly Guid Name = new Guid("{D0D16375-2E92-4400-BD12-C8CBEE327CE6}");

        public static readonly Guid TemplateId = new Guid("{CBBB407D-C27B-49C1-ABB0-D9353669B15A}");

        public static readonly Guid FieldVersioning = new Guid("{BE351A73-FCB0-4213-93FA-C302D8AB4F51}");

        #endregion
    }

    public static class TemplateFieldIDs
    {
        #region Constants

        public static readonly ID DefaultValue = new ID("{B118496A-0F78-4A27-B55F-0A6C4B0B0FE1}");

        public static readonly ID Description = new ID("{577F1689-7DE4-4AD2-A15F-7FDC1759285F}");

        public static readonly ID HelpLink = new ID("{56776EDF-261C-4ABC-9FE7-70C618795239}");

        public static readonly ID Blob = new ID("{FF8A2D01-8A77-4F1B-A966-65806993CD31}");

        public static readonly ID ResetBlank = new ID("{BF110AA2-F46C-4B8C-8FA5-FA4CEACCF99D}");

        public static readonly ID Shared = new ID("{BE351A73-FCB0-4213-93FA-C302D8AB4F51}");

        public static readonly ID Source = new ID("{1EB8AE32-E190-44A6-968D-ED904C794EBF}");

        public static readonly ID Title = new ID("{19A69332-A23E-4E70-8D16-B2640CB24CC8}");

        public static readonly ID Type = new ID("{AB162CC0-DC80-4ABF-8871-998EE5D7BA32}");

        public static readonly ID ToolTip = new ID("{9541E67D-CE8C-4225-803D-33F7F29F09EF}");

        public static readonly ID Unversioned = new ID("{39847666-389D-409B-95BD-F2016F11EED5}");

        public static readonly ID Validation = new ID("{074F44CA-359A-4C13-B3AA-4A6BE2A675B1}");

        public static readonly ID ValidationText = new ID("{B12E4906-B96B-495E-B343-CD2E92DC6347}");

        public static readonly ID ExcludeFromTextSearch = new ID("{9CD5874F-0EED-481C-8E75-AE162E613B59}");

        public static readonly ID DashboardReportNameFieldId = new ID("{49FA57D8-E607-49F5-BB07-981B8FE5E005}");

        public static readonly ID DashboardExpirationTimeFieldId = new ID("{8E812C23-F893-4842-9833-97A32461D7A5}");

        #endregion
    }

    public static class TemplateIDs
    {
        #region Constants

        public static readonly ID Application = new ID("{EB06CEC0-5E2D-4DC4-875B-01ADCC577D13}");

        public static readonly ID Alias = new ID("{54BCFFB7-8F46-4948-AE74-DA5B6B5AFA86}");

        public static readonly ID ArchivedItem = new ID("{BF2B8DA2-3CBA-485D-8F85-3788B8AFBDBF}");

        public static readonly ID Command = new ID("{A66F4A32-23A6-4AC3-AB14-84F383C5F3BA}");

        public static readonly ID CommandGroup = new ID("{FBDD7D4F-3300-4432-9CB1-FDAD551DDD5E}");

        public static readonly ID CommandMaster = new ID("{B2613CC1-A748-46A3-A0DB-3774574BD339}");

        public static readonly ID Device = new ID("{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}");

        public static readonly ID DictionaryEntry = new ID("{6D1CD897-1936-4A3A-A511-289A94C2A7B1}");

        public static readonly ID DictionaryDomain = new ID("{0A2847E6-9885-450B-B61E-F9E6528480EF}");

        public static readonly ID Domain = new ID("{438F33BA-504D-4EFB-BE78-42B98603A7E8}");

        public static readonly ID DynamicMaster = new ID("{B4D19D07-B3EB-4F7D-98EC-8BCB41CCC58E}");

        public static readonly ID Folder = new ID("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}");

        public static readonly ID HelpText = new ID("{11A111FF-4343-4AF8-91DC-3C49DBEDD9C6}");

        public static readonly ID Image = new ID("{C97BA923-8009-4858-BDD5-D8BE5FCCECF7}");

        public static readonly ID Language = new ID("{F68F13A6-3395-426A-B9A1-FA2DC60D94EB}");

        public static readonly ID Layout = new ID("{3A45A723-64EE-4919-9D41-02FD40FD1466}");

        public static readonly ID LayoutGroup = new ID("{85B7F5D9-6F3A-4AD1-B218-D9FBE1DF8BB3}");

        public static readonly ID LinkedDatabase = new ID("{2085BFB3-1371-4092-BF06-32EF5849ED87}");

        public static readonly ID MainSection = new ID("{E3E2D58C-DF95-4230-ADC9-279924CECE84}");

        public static readonly ID BranchTemplate = new ID("{35E75C72-4985-4E09-88C3-0EAC6CD1E64F}");

        public static readonly ID BranchTemplateFolder = new ID("{85ADBF5B-E836-4932-A333-FE0F9FA1ED1E}");

        public static readonly ID MediaFolder = new ID("{FE5DD826-48C6-436D-B87A-7C4210C7413B}");

        public static readonly ID MenuDivider = new ID("{35753BF3-8A94-4DA6-A5D7-DBD945D1AA59}");

        public static readonly ID MenuItem = new ID("{998B965E-6AB8-4568-810F-8101D60D0CC3}");

        public static readonly ID Node = new ID("{239F9CF4-E5A0-44E0-B342-0F32CD4C6D8B}");

        public static readonly ID Notification = new ID("{35ABD142-200F-43F7-ADB6-0E10EDB82C0B}");

        public static readonly ID PackageRegistration = new ID("{22A11D20-5F1D-4216-BF3F-18C016F1F98E}");

        public static readonly ID Property = new ID("{97D75760-CF8B-4740-810B-7727B564EF4D}");

        public static readonly ID Proxy = new ID("{CB3942DC-CBBA-4332-A7FB-C4E4204C538A}");

        public static readonly ID Reference = new ID("{EF295CD8-19D4-4E02-9438-94C926EF5284}");

        public static readonly ID RenderingGroup = new ID("{4B8AD536-4FA6-4122-A31E-32BB6BC42806}");

        public static readonly ID Role = new ID("{A7DF04B4-4C4B-44B7-BE1E-AD901BD53DAD}");

        public static readonly ID Schedule = new ID("{70244923-FA84-477C-8CBD-62F39642C42B}");

        public static readonly ID StandardTemplate = new ID("{1930BBEB-7805-471A-A3BE-4858AC7CF696}");

        public static readonly ID Sublayout = new ID("{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}");

        public static readonly ID Template = new ID("{AB86861A-6030-46C5-B394-E8F99E8B87DB}");

        public static readonly ID TemplateField = new ID("{455A3E98-A627-4B40-8035-E683A0331AC7}");

        public static readonly ID TemplateFieldType = new ID("{F8A17D6A-118E-4CD7-B5F5-88FF37A4F237}");

        public static readonly ID TemplateFolder = new ID("{0437FEE2-44C9-46A6-ABE9-28858D9FEE8C}");

        public static readonly ID TemplateSection = new ID("{E269FBB5-3750-427A-9149-7AA950B49301}");

        public static readonly ID User = new ID("{642C9A7E-EE31-4979-86F0-39F338C10AFB}");

        public static readonly ID VersionedImage = new ID("{C97BA923-8009-4858-BDD5-D8BE5FCCECF7}");

        public static readonly ID UnversionedImage = new ID("{F1828A2C-7E5D-4BBD-98CA-320474871548}");

        public static readonly ID Workflow = new ID("{1C0ACC50-37BE-4742-B43C-96A07A7410A5}");

        public static readonly ID WorkflowCommand = new ID("{CB01F9FC-C187-46B3-AB0B-97A8468D8303}");

        public static readonly ID WorkflowState = new ID("{4B7E2DA9-DE43-4C83-88C3-02F042031D04}");

        public static readonly ID XSLRendering = new ID("{F1F1D639-4F54-40C2-8BE0-81266B392CEB}");

        public static readonly ID XMLLayout = new ID("{1163DA83-B2EF-4381-BF09-B2FF714B1B3F}");

        public static readonly ID File = new ID("{611933AC-CE0C-4DDC-9683-F830232DB150}");

        public static readonly ID PresetPersona = new ID("{448C4D57-F0F1-4278-8AB8-93DB46C94F1B}");

        public static readonly ID Preset = new ID("{0FC09EA4-8D87-4B0E-A5C9-8076AE863D9C}");

        #endregion
    }

    public static class TemplateSectionFieldIDs
    {
        #region Constants

        public static readonly ID Control = new ID("{066D3A48-A989-4CF4-8E2E-9A892DFB2012}");

        public static readonly ID HideFields = new ID("{930FD271-03C7-4E6D-9C41-E124FA1D4375}");

        public static readonly ID HiddenByDefault = new ID("{3117AA30-F614-4B2E-9428-28EC4166FABE}");

        #endregion
    }

    public static class SecurityDatabaseTemplateIDs
    {
        #region Constants

        public static readonly ID Folder = new ID("{AAD4C04A-EAA6-4824-87D2-E01F2325D422}");

        #endregion
    }

    public static class RoleFieldIDs
    {
        public static readonly ID Roles = new ID("{CCB11355-CD36-4785-BFFA-4F12484348EA}");
    }

    public static class UserFieldIDs
    {
        public static readonly ID Administrator = new ID("{C167BE37-E450-42CA-99F3-ED82C1897D26}");

        public static readonly ID Password = new ID("{BB003F3F-1C68-4D36-90AF-39B568B94DFE}");

        public static readonly ID Roles = new ID("{F7115A7D-07A2-42CE-8B38-D64EA4CB6124}");
    }

    public static class WorkflowFieldIDs
    {
        public static readonly ID FinalState = new ID("{FB8ABC73-7ACF-45A0-898C-D3CCB889C3EE}");

        public static ID PreviewPublishingTargets = new ID("{A438DFD1-154E-4584-B7E3-D1E28315E415}");
    }

    public static class RenderingIDs
    {
        public static readonly ID FieldRenderer = new ID("{E1AF4AA3-3B5D-4611-8C71-959AD261E5B7}");

        public static readonly ID HiddenRendering = new ID("{0777B05D-6AC0-44BE-9605-052E0DB5993D}");

        public static readonly ID WebEditContentEditor = new ID("{683331AD-FF41-442C-9CFE-9872C0A7150E}");

        public static readonly ID WebEditRibbon = new ID("{054FE55F-E3FB-46C6-8CB0-948F368B6579}");
    }

    public static class ItemBucketIDs
    {
        public static readonly ID Section = new ID("{AF530C7B-8B87-458B-80CE-239D1E1B9E60}");
    }
}
