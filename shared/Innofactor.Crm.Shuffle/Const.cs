// *********************************************************************
// Created by : Latebound Constants Generator 1.2020.2.1 for XrmToolBox
// Author     : Jonas Rapp https://twitter.com/rappen
// GitHub     : https://github.com/rappen/LCG-UDG
// Source Org : https://crmk-bn-test.crm4.dynamics.com/
// Filename   : C:\Dev\GitHub\Shuffle\Innofactor.Crm.CI\shared\Innofactor.Crm.Shuffle\Const.cs
// Created    : 2020-12-02 22:12:30
// *********************************************************************

namespace Cinteros.Crm.Utils.Shuffle
{
    /// <summary>DisplayName: Import Job, OwnershipType: OrganizationOwned, IntroducedVersion: 5.0.0.0</summary>
    public static class ImportJob
    {
        public const string EntityName = "importjob";
        public const string EntityCollectionName = "importjobs";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        public const string PrimaryKey = "importjobid";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CompletedOn = "completedon";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string CreatedBy = "createdby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CreatedOn = "createdon";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 1073741823, Format: Text</summary>
        public const string Data = "data";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 110, Format: Text</summary>
        public const string ImportContext = "importcontext";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string ImportJobName = "name";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: Lookup, RequiredLevel: SystemRequired, Targets: organization</summary>
        public const string Organization = "organizationid";
        /// <summary>Type: Double, RequiredLevel: None, MinValue: 0, MaxValue: 100, Precision: 2</summary>
        public const string Progress = "progress";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string Solution = "solutionid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 50, Format: Text</summary>
        public const string SolutionOperationContext = "operationcontext";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string SolutionName = "solutionname";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string StartedOn = "startedon";
    }

    /// <summary>DisplayName: System Job, OwnershipType: UserOwned, IntroducedVersion: 5.0.0.0</summary>
    public static class SystemJob
    {
        public const string EntityName = "asyncoperation";
        public const string EntityCollectionName = "asyncoperations";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        public const string PrimaryKey = "asyncoperationid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string PrimaryName = "name";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string BreadcrumbID = "breadcrumbid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string CallerOrigin = "callerorigin";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CompletedOn = "completedon";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        public const string CorrelationId = "correlationid";
        /// <summary>Type: DateTime, RequiredLevel: SystemRequired, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CorrelationUpdatedTime = "correlationupdatedtime";
        /// <summary>Type: Lookup, RequiredLevel: SystemRequired, Targets: systemuser</summary>
        public const string CreatedBy = "createdby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CreatedOn = "createdon";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 1073741823</summary>
        public const string Data = "data";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string DependencyToken = "dependencytoken";
        /// <summary>Type: Integer, RequiredLevel: SystemRequired, MinValue: 0, MaxValue: 2147483647</summary>
        public const string Depth = "depth";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string ErrorCode = "errorcode";
        /// <summary>Type: Double, RequiredLevel: SystemRequired, MinValue: 0, MaxValue: 1000000000, Precision: 2</summary>
        public const string ExecutionTimeSpan = "executiontimespan";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string ExpanderStartTime = "expanderstarttime";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 100000</summary>
        public const string Friendlymessage = "friendlymessage";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string Host = "hostid";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 100000</summary>
        public const string Message = "message";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 160, Format: Text</summary>
        public const string MessageName = "messagename";
        /// <summary>Type: Lookup, RequiredLevel: SystemRequired, Targets: systemuser</summary>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        public const string Owner = "ownerid";
        /// <summary>Type: EntityName, RequiredLevel: SystemRequired</summary>
        public const string owneridtype = "owneridtype";
        /// <summary>Type: Lookup, RequiredLevel: SystemRequired, Targets: businessunit</summary>
        public const string OwningBusinessUnit = "owningbusinessunit";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: sdkmessageprocessingstep</summary>
        public const string OwningExtension = "owningextensionid";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string PostponeUntil = "postponeuntil";
        /// <summary>Type: EntityName, RequiredLevel: None, DisplayName: Primary Entity Type, OptionSetType: Picklist</summary>
        public const string PrimaryEntityType = "primaryentitytype";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string RecurrencePattern = "recurrencepattern";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        public const string RecurrenceStart = "recurrencestarttime";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: account,accountleads,activitymimeattachment,activitymonitor,activitypointer,adminsettingsentity,annotation,annualfiscalcalendar,apisettings,appelement,applicationuser,appmodulecomponentedge,appmodulecomponentnode,appointment,appsetting,attributeimageconfig,attributemap,bookableresource,bookableresourcebooking,bookableresourcebookingexchangesyncidmapping,bookableresourcebookingheader,bookableresourcecategory,bookableresourcecategoryassn,bookableresourcecharacteristic,bookableresourcegroup,bookingstatus,bot,botcomponent,bulkoperation,bulkoperationlog,businessunit,businessunitnewsarticle,calendar,campaign,campaignactivity,campaignactivityitem,campaignitem,campaignresponse,canvasappextendedmetadata,cascadegrantrevokeaccessrecordstracker,cascadegrantrevokeaccessversiontracker,catalog,catalogassignment,channelaccessprofile,channelaccessprofilerule,characteristic,childincidentcount,commitment,competitor,competitoraddress,competitorproduct,competitorsalesliterature,connection,connectionreference,connectionrole,connector,constraintbasedgroup,contact,contactinvoices,contactleads,contactorders,contactquotes,contract,contractdetail,contracttemplate,conversationtranscript,convertrule,crmk_bisnodeauthentication,crmk_bisnodeconfiguration,crmk_bisnodecreditlog,crmk_bisnodedummyprocess,crmk_bisnodemapping,crmk_bisnodesynchronization,crmk_bisnodesynclog,crmk_licensekey,crmk_localizedlanguage,crmk_localizedtext,crmk_localizedtranslation,crmk_variable,customapi,customapirequestparameter,customapiresponseproperty,customeraddress,customeropportunityrole,customerrelationship,datalakefolder,datalakefolderpermission,datalakeworkspace,datalakeworkspacepermission,discount,discounttype,displaystring,dynamicproperty,dynamicpropertyassociation,dynamicpropertyinstance,dynamicpropertyoptionsetitem,email,emailserverprofile,entitlement,entitlementchannel,entitlementcontacts,entitlemententityallocationtypemapping,entitlementproducts,entitlementtemplate,entitlementtemplatechannel,entitlementtemplateproducts,entityanalyticsconfig,entityimageconfig,entitymap,environmentvariabledefinition,environmentvariablevalue,equipment,exportsolutionupload,externalparty,externalpartyitem,fax,fixedmonthlyfiscalcalendar,flowsession,goal,goalrollupquery,holidaywrapper,import,importdata,importfile,importlog,importmap,incident,incidentknowledgebaserecord,incidentresolution,interactionforemail,internalcatalogassignment,invoice,invoicedetail,isvconfig,kbarticle,kbarticlecomment,kbarticletemplate,knowledgearticle,knowledgearticleincident,knowledgebaserecord,lead,leadaddress,leadcompetitors,leadproduct,leadtoopportunitysalesprocess,letter,list,listmember,listoperation,mailbox,mailmergetemplate,metric,monthlyfiscalcalendar,msdynce_botcontent,msdyn_actioncardregarding,msdyn_actioncardrolesetting,msdyn_adaptivecardconfiguration,msdyn_aibdataset,msdyn_aibdatasetfile,msdyn_aibdatasetrecord,msdyn_aibdatasetscontainer,msdyn_aibfile,msdyn_aibfileattacheddata,msdyn_aiconfiguration,msdyn_aifptrainingdocument,msdyn_aimodel,msdyn_aiodimage,msdyn_aiodlabel,msdyn_aiodtrainingboundingbox,msdyn_aiodtrainingimage,msdyn_aitemplate,msdyn_analysiscomponent,msdyn_analysisjob,msdyn_analysisresult,msdyn_analysisresultdetail,msdyn_analyticsadminsettings,msdyn_analyticsforcs,msdyn_appconfiguration,msdyn_applicationextension,msdyn_applicationtabtemplate,msdyn_assetcategorytemplateassociation,msdyn_assettemplateassociation,msdyn_autocapturerule,msdyn_autocapturesettings,msdyn_callablecontext,msdyn_caseenrichment,msdyn_casesuggestionrequestpayload,msdyn_casetopic,msdyn_casetopicsetting,msdyn_casetopicsummary,msdyn_casetopic_incident,msdyn_channelprovider,msdyn_collabgraphresource,msdyn_customerasset,msdyn_customerassetattachment,msdyn_customerassetcategory,msdyn_dataanalyticsreport,msdyn_dataanalyticsreport_csrmanager,msdyn_dataanalyticsreport_ksinsights,msdyn_databaseversion,msdyn_dataflow,msdyn_datainsightsandanalyticsfeature,msdyn_entityrankingrule,msdyn_federatedarticle,msdyn_federatedarticleincident,msdyn_flowcardtype,msdyn_forecastconfiguration,msdyn_forecastdefinition,msdyn_forecastinstance,msdyn_forecastrecurrence,msdyn_functionallocation,msdyn_helppage,msdyn_icebreakersconfig,msdyn_iotalert,msdyn_iotdevice,msdyn_iotdevicecategory,msdyn_iotdevicecommand,msdyn_iotdevicecommanddefinition,msdyn_iotdevicedatahistory,msdyn_iotdeviceproperty,msdyn_iotdeviceregistrationhistory,msdyn_iotdevicevisualizationconfiguration,msdyn_iotfieldmapping,msdyn_iotpropertydefinition,msdyn_iotprovider,msdyn_iotproviderinstance,msdyn_iotsettings,msdyn_iottocaseprocess,msdyn_kbenrichment,msdyn_kmfederatedsearchconfig,msdyn_knowledgearticleimage,msdyn_knowledgearticletemplate,msdyn_knowledgeinteractioninsight,msdyn_knowledgesearchinsight,msdyn_macrosession,msdyn_migrationtracker,msdyn_msteamssetting,msdyn_msteamssettingsv2,msdyn_notesanalysisconfig,msdyn_notificationfield,msdyn_notificationtemplate,msdyn_paneconfiguration,msdyn_panetabconfiguration,msdyn_panetoolconfiguration,msdyn_playbookactivity,msdyn_playbookactivityattribute,msdyn_playbookcategory,msdyn_playbookinstance,msdyn_playbooktemplate,msdyn_postalbum,msdyn_postconfig,msdyn_postruleconfig,msdyn_productivityactioninputparameter,msdyn_productivityactionoutputparameter,msdyn_productivityagentscript,msdyn_productivityagentscriptstep,msdyn_productivitymacroactiontemplate,msdyn_productivitymacroconnector,msdyn_productivitymacrosolutionconfiguration,msdyn_productivityparameterdefinition,msdyn_property,msdyn_propertyassetassociation,msdyn_propertylog,msdyn_propertytemplateassociation,msdyn_relationshipinsightsunifiedconfig,msdyn_richtextfile,msdyn_salesinsightssettings,msdyn_serviceconfiguration,msdyn_sessiontemplate,msdyn_siconfig,msdyn_sikeyvalueconfig,msdyn_slakpi,msdyn_smartassistconfig,msdyn_solutionhealthrule,msdyn_solutionhealthruleargument,msdyn_solutionhealthruleset,msdyn_suggestioninteraction,msdyn_suggestionrequestpayload,msdyn_suggestionsmodelsummary,msdyn_suggestionssetting,msdyn_teamscollaboration,msdyn_templateforproperties,msdyn_templateparameter,msdyn_untrackedappointment,msdyn_upgraderun,msdyn_upgradestep,msdyn_upgradeversion,msdyn_wallsavedquery,msdyn_wallsavedqueryusersettings,msfp_alert,msfp_alertrule,msfp_emailtemplate,msfp_fileresponse,msfp_localizedemailtemplate,msfp_project,msfp_question,msfp_questionresponse,msfp_satisfactionmetric,msfp_survey,msfp_surveyinvite,msfp_surveyreminder,msfp_surveyresponse,msfp_unsubscribedrecipient,opportunity,opportunityclose,opportunitycompetitors,opportunityproduct,opportunitysalesprocess,orderclose,organization,package,pdfsetting,phonecall,phonetocaseprocess,position,post,postfollow,pricelevel,privilege,processstageparameter,product,productassociation,productpricelevel,productsalesliterature,productsubstitute,provisionlanguageforuser,quarterlyfiscalcalendar,queue,queueitem,quote,quoteclose,quotedetail,ratingmodel,ratingvalue,recurringappointmentmaster,relationshipattribute,relationshiprole,relationshiprolemap,report,resource,resourcegroup,resourcegroupexpansion,resourcespec,revokeinheritedaccessrecordstracker,role,rollupfield,routingrule,routingruleitem,salesliterature,salesliteratureitem,salesorder,salesorderdetail,salesprocessinstance,savedquery,semiannualfiscalcalendar,service,serviceappointment,servicecontractcontacts,serviceplan,settingdefinition,sharepointdocumentlocation,sharepointsite,similarityrule,site,sla,socialactivity,socialprofile,solutioncomponentattributeconfiguration,solutioncomponentconfiguration,solutioncomponentrelationshipconfiguration,stagesolutionupload,subject,systemform,systemuser,task,team,teammobileofflineprofilemembership,template,territory,theme,topic,topichistory,topicmodel,topicmodelconfiguration,topicmodelexecutionhistory,transactioncurrency,uom,uomschedule,userform,usermapping,usermobileofflineprofilemembership,userquery,workflowbinary</summary>
        public const string Regarding = "regardingobjectid";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string Request = "requestid";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string RetainJobHistory = "retainjobhistory";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string RetryCount = "retrycount";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 1073741823</summary>
        public const string RootExecutionContext = "rootexecutioncontext";
        /// <summary>Type: BigInt, RequiredLevel: SystemRequired, MinValue: -9223372036854775808, MaxValue: 9223372036854775807</summary>
        public const string Sequence = "sequence";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string StartedOn = "startedon";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Status, OptionSetType: State</summary>
        public const string Status = "statecode";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Status Reason, OptionSetType: Status</summary>
        public const string StatusReason = "statuscode";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 255</summary>
        public const string Subtype = "subtype";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: System Job Type, OptionSetType: Picklist, DefaultFormValue: -1</summary>
        public const string SystemJobType = "operationtype";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string WaitingforEvent = "iswaitingforevent";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: workflow</summary>
        public const string WorkflowActivationId = "workflowactivationid";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string WorkflowIsBlocked = "workflowisblocked";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string WorkflowStage = "workflowstagename";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 1073741823, Format: TextArea</summary>
        public const string WorkflowState = "workflowstate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string Workload = "workload";
        public enum owneridtype_OptionSet
        {
        }
        public enum PrimaryEntityType_OptionSet
        {
        }
        public enum Status_OptionSet
        {
            Ready = 0,
            Suspended = 1,
            Locked = 2,
            Completed = 3
        }
        public enum StatusReason_OptionSet
        {
            WaitingForResources = 0,
            Waiting = 10,
            InProgress = 20,
            Pausing = 21,
            Canceling = 22,
            Succeeded = 30,
            Failed = 31,
            Canceled = 32
        }
        public enum SystemJobType_OptionSet
        {
            SystemEvent = 1,
            BulkEmail = 2,
            ImportFileParse = 3,
            TransformParseData = 4,
            Import = 5,
            ActivityPropagation = 6,
            DuplicateDetectionRulePublish = 7,
            BulkDuplicateDetection = 8,
            SQMDataCollection = 9,
            Workflow = 10,
            QuickCampaign = 11,
            MatchcodeUpdate = 12,
            BulkDelete = 13,
            DeletionService = 14,
            IndexManagement = 15,
            CollectOrganizationStatistics = 16,
            ImportSubprocess = 17,
            CalculateOrganizationStorageSize = 18,
            CollectOrganizationDatabaseStatistics = 19,
            CollectionOrganizationSizeStatistics = 20,
            DatabaseTuning = 21,
            CalculateOrganizationMaximumStorageSize = 22,
            BulkDeleteSubprocess = 23,
            UpdateStatisticIntervals = 24,
            OrganizationFullTextCatalogIndex = 25,
            Databaselogbackup = 26,
            UpdateContractStates = 27,
            DBCCSHRINKDATABASEmaintenancejob = 28,
            DBCCSHRINKFILEmaintenancejob = 29,
            Reindexallindicesmaintenancejob = 30,
            StorageLimitNotification = 31,
            Cleanupinactiveworkflowassemblies = 32,
            RecurringSeriesExpansion = 35,
            ImportSampleData = 38,
            GoalRollUp = 40,
            AuditPartitionCreation = 41,
            CheckForLanguagePackUpdates = 42,
            ProvisionLanguagePack = 43,
            UpdateOrganizationDatabase = 44,
            UpdateSolution = 45,
            RegenerateEntityRowCountSnapshotData = 46,
            RegenerateReadShareSnapshotData = 47,
            OutgoingActivity = 50,
            IncomingEmailProcessing = 51,
            MailboxTestAccess = 52,
            EncryptionHealthCheck = 53,
            ExecuteAsyncRequest = 54,
            PosttoYammer = 49,
            UpdateEntitlementStates = 56,
            CalculateRollupField = 57,
            MassCalculateRollupField = 58,
            ImportTranslation = 59,
            ConvertDateAndTimeBehavior = 62,
            EntityKeyIndexCreation = 63,
            UpdateKnowledgeArticleStates = 65,
            ResourceBookingSync = 68,
            RelationshipAssistantCards = 69,
            CleanupSolutionComponents = 71,
            AppModuleMetadataOperation = 72,
            ALMAnomalyDetectionOperation = 73,
            FlowNotification = 75,
            RibbonClientMetadataOperation = 76,
            CallbackRegistrationExpanderOperation = 79,
            CascadeAssign = 90,
            CascadeDelete = 91,
            EventExpanderOperation = 92,
            ImportSolutionMetadata = 93,
            BulkDeleteFileAttachment = 94,
            RefreshBusinessUnitforRecordsOwnedByPrincipal = 95,
            RevokeInheritedAccess = 96,
            CascadeGrantorRevokeAccessVersionTrackingAsyncOperation = 12801,
            AIBuilderTrainingEvents = 190690091,
            AIBuilderPredictionEvents = 190690092,
            Provisionlanguageforuser = 201,
            CreateOrRefreshVirtualEntity = 98
        }
    }
}
