// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.2.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://jonassandbox.crm4.dynamics.com/
// Filename  : C:\Dev\Git\DevUtils\Crm.Utils.Shuffle\Const.cs
// Created   : 2018-04-19 12:39:24
// *********************************************************************

namespace Cinteros.Crm.Utils.Shuffle
{
    /// <summary>DisplayName: Import Job, OwnershipType: OrganizationOwned, IntroducedVersion: 5.0.0.0</summary>
    public static class ImportJob
    {
        public const string EntityName = "importjob";
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
        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        public const string PrimaryKey = "asyncoperationid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string PrimaryName = "name";
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
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: account,accountleads,activitymimeattachment,activitypointer,annotation,annualfiscalcalendar,appointment,attributemap,bookableresource,bookableresourcebooking,bookableresourcebookingexchangesyncidmapping,bookableresourcebookingheader,bookableresourcecategory,bookableresourcecategoryassn,bookableresourcecharacteristic,bookableresourcegroup,bookingstatus,bulkoperation,bulkoperationlog,businessunit,businessunitnewsarticle,calendar,campaign,campaignactivity,campaignactivityitem,campaignitem,campaignresponse,channelaccessprofile,channelaccessprofilerule,characteristic,childincidentcount,cint_absence,cint_activitybase,cint_actual_plugin_step,cint_aggregation,cint_autonum,cint_autonumlock,cint_cascade,cint_configuration,cint_course,cint_course_answer,cint_course_category,cint_course_date,cint_course_document,cint_course_document_type,cint_course_kit,cint_course_material,cint_course_question,cint_course_reservation,cint_course_reservation_history,cint_course_reservation_voucher,cint_course_session,cint_course_type,cint_cws_answer,cint_cws_layout,cint_cws_option,cint_cws_question,cint_cws_respondeetype,cint_cws_survey,cint_cws_takensurvey,cint_discount_contract,cint_dynamic_view,cint_dynamic_view_param,cint_invoice_data,cint_invoice_details,cint_invoice_item,cint_invoice_run,cint_language,cint_lng_text,cint_lng_translation,cint_lookup,cint_membership,cint_membership_base,cint_membership_product,cint_msg,cint_msg_group,cint_msg_text,cint_participant_property,cint_participant_property_base,cint_payment_term,cint_plugin_config_batch,cint_qualification,cint_reason,cint_rpt_relation,cint_rpt_type,cint_server_side_request,cint_session,cint_session_reservation,cint_skill,cint_teacher_property,cint_teacher_property_base,cint_teacher_requirement,cint_teacher_reservation,cint_teacher_session_reservation,cint_teacher_type,cint_track,cint_track_session,cint_treeview,cint_treeviewnode,cint_treeviewnoderelation,cint_venue,cint_venue_attribute,cint_venue_booking,cint_venue_category,cint_venue_property,cint_venue_requirement,cint_voucher,commitment,competitor,competitoraddress,competitorproduct,competitorsalesliterature,connection,connectionrole,constraintbasedgroup,contact,contactinvoices,contactleads,contactorders,contactquotes,contract,contractdetail,contracttemplate,convertrule,customeraddress,customeropportunityrole,customerrelationship,discount,discounttype,displaystring,dynamicproperty,dynamicpropertyassociation,dynamicpropertyinstance,dynamicpropertyoptionsetitem,email,emailserverprofile,entitlement,entitlementchannel,entitlementcontacts,entitlementproducts,entitlementtemplate,entitlementtemplatechannel,entitlementtemplateproducts,entitymap,equipment,externalparty,externalpartyitem,fax,fixedmonthlyfiscalcalendar,goal,goalrollupquery,import,importdata,importfile,importlog,importmap,incident,incidentknowledgebaserecord,incidentresolution,interactionforemail,invoice,invoicedetail,isvconfig,kbarticle,kbarticlecomment,kbarticletemplate,knowledgearticle,knowledgearticleincident,knowledgebaserecord,lead,leadaddress,leadcompetitors,leadproduct,leadtoopportunitysalesprocess,letter,list,listmember,mailbox,mailmergetemplate,metric,monthlyfiscalcalendar,msdyn_accountpricelist,msdyn_actual,msdyn_approval,msdyn_batchjob,msdyn_bookingalert,msdyn_bookingalertstatus,msdyn_bookingchange,msdyn_bookingrule,msdyn_bookingsetupmetadata,msdyn_bpf_665e73aa18c247d886bfc50499c73b82,msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d,msdyn_characteristicreqforteammember,msdyn_clientextension,msdyn_configuration,msdyn_contactpricelist,msdyn_contractlineinvoiceschedule,msdyn_contractlinescheduleofvalue,msdyn_dataexport,msdyn_delegation,msdyn_estimate,msdyn_estimateline,msdyn_expense,msdyn_expensecategory,msdyn_expensereceipt,msdyn_fact,msdyn_fieldcomputation,msdyn_findworkevent,msdyn_integrationjob,msdyn_integrationjobdetail,msdyn_invoicefrequency,msdyn_invoicefrequencydetail,msdyn_invoicelinetransaction,msdyn_journal,msdyn_journalline,msdyn_mlresultcache,msdyn_opportunitylineresourcecategory,msdyn_opportunitylinetransaction,msdyn_opportunitylinetransactioncategory,msdyn_opportunitylinetransactionclassificatio,msdyn_opportunitypricelist,msdyn_orderlineresourcecategory,msdyn_orderlinetransaction,msdyn_orderlinetransactioncategory,msdyn_orderlinetransactionclassification,msdyn_orderpricelist,msdyn_organizationalunit,msdyn_postalbum,msdyn_postconfig,msdyn_postruleconfig,msdyn_priority,msdyn_processnotes,msdyn_project,msdyn_projectapproval,msdyn_projectparameter,msdyn_projectparameterpricelist,msdyn_projectpricelist,msdyn_projecttask,msdyn_projecttaskdependency,msdyn_projecttaskstatususer,msdyn_projectteam,msdyn_projectteammembersignup,msdyn_projecttransactioncategory,msdyn_quotelineanalyticsbreakdown,msdyn_quotelineinvoiceschedule,msdyn_quotelineresourcecategory,msdyn_quotelinescheduleofvalue,msdyn_quotelinetransaction,msdyn_quotelinetransactioncategory,msdyn_quotelinetransactionclassification,msdyn_quotepricelist,msdyn_relationshipinsightsunifiedconfig,msdyn_requirementcharacteristic,msdyn_requirementorganizationunit,msdyn_requirementresourcecategory,msdyn_requirementresourcepreference,msdyn_requirementstatus,msdyn_resourceassignment,msdyn_resourceassignmentdetail,msdyn_resourcecategorypricelevel,msdyn_resourcerequest,msdyn_resourcerequirement,msdyn_resourcerequirementdetail,msdyn_resourceterritory,msdyn_rolecompetencyrequirement,msdyn_roleutilization,msdyn_scheduleboardsetting,msdyn_schedulingparameter,msdyn_siconfig,msdyn_systemuserschedulersetting,msdyn_timeentry,msdyn_timegroup,msdyn_timegroupdetail,msdyn_timeoffcalendar,msdyn_transactioncategory,msdyn_transactioncategoryclassification,msdyn_transactioncategoryhierarchyelement,msdyn_transactioncategorypricelevel,msdyn_transactionconnection,msdyn_transactionorigin,msdyn_transactiontype,msdyn_userworkhistory,msdyn_wallsavedquery,msdyn_wallsavedqueryusersettings,msdyn_workhourtemplate,opportunity,opportunityclose,opportunitycompetitors,opportunityproduct,opportunitysalesprocess,orderclose,organization,phonecall,phonetocaseprocess,position,post,postfollow,pricelevel,privilege,product,productassociation,productpricelevel,productsalesliterature,productsubstitute,quarterlyfiscalcalendar,queue,queueitem,quote,quoteclose,quotedetail,ratingmodel,ratingvalue,recurringappointmentmaster,relationshiprole,relationshiprolemap,report,resource,resourcegroup,resourcegroupexpansion,resourcespec,role,rollupfield,routingrule,routingruleitem,salesliterature,salesliteratureitem,salesorder,salesorderdetail,salesprocessinstance,savedquery,semiannualfiscalcalendar,service,serviceappointment,servicecontractcontacts,sharepointdocumentlocation,sharepointsite,similarityrule,site,sla,socialactivity,socialprofile,subject,systemform,systemuser,task,team,template,territory,theme,topic,topichistory,topicmodel,topicmodelconfiguration,topicmodelexecutionhistory,transactioncurrency,uom,uomschedule,userform,usermapping,userquery</summary>
        public const string Regarding = "regardingobjectid";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string Request = "requestid";
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
            ALMAnomalyDetectionOperation = 73
        }
    }
}
