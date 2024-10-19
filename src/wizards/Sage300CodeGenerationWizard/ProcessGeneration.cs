﻿// The MIT License (MIT) 
// Copyright (c) 1994-2024 The Sage Group plc or its licensors.  All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of 
// this software and associated documentation files (the "Software"), to deal in 
// the Software without restriction, including without limitation the rights to use, 
// copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#region Imports
using ACCPAC.Advantage;
using Sage.CA.SBS.ERP.Sage300.CodeGenerationWizard.Properties;
using Sage.CA.SBS.ERP.Sage300.CodeGenerationWizard.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using View = ACCPAC.Advantage.View;
using System.Xml;
using EnvDTE;
using Newtonsoft.Json;
using Jint.Runtime;
using Newtonsoft.Json.Linq;
using Thread = System.Threading.Thread;
using static Microsoft.VisualStudio.VSConstants;

#endregion

namespace Sage.CA.SBS.ERP.Sage300.CodeGenerationWizard
{
    /// <summary> Enumeration for solution type (web or web api) </summary>
    public enum WizardType
    {
        /// <summary> Web solution type </summary>
        WEB = 0,
        /// <summary> Web API solution type </summary>
        WEBAPI = 1
    }

    /// <summary> Process Generation Class (worker) </summary>
    class ProcessGeneration
    {
#region Private Variables

        /// <summary> Settings from UI </summary>
        private Settings _settings;

#endregion

#region Private Constants
        private static class PrivateConstants
        {
            public const string RootEnumerationsFilename = "Enumerations.cs";
            public const string CommonEnumerationsFilename = "CommonEnumerations.cs";
            public const string ProcessEnumerationsFilename = "Enumerations.cs";
            public const string ReportEnumerationsFilename = "Enumerations.cs";
            public const string CustomCommonResx = "CustomCommonResx";
        }
#endregion

#region Public Constants
        public static class Constants
        {
            /// <summary> SettingsKey is used as a dictionary key for settings </summary>
            public const string SettingsKey = "settings";

            /// <summary> WebApiKey is used as a dictionary key for projects </summary>
            public const string WebApiKey = "WebApi";

            /// <summary> subfolder for WebApi Controller </summary>
            public const string SubfolderWebApiControllerKey = "Controllers";

            /// <summary> subfolder for WebApi versioning </summary>
            public const string SubfolderWebApiVersioningKey = "Versioning";

            /// <summary> WebApiModelsKey is used as a dictionary key for projects </summary>
            public const string WebApiModelsKey = "WebApi.Models";

            /// <summary> subfolder for WebApi.Models</summary>
            public const string SubfolderWebApiModelsCustomKey = "Custom";


            /// <summary> subfolder for WebApi.Models</summary>
            public const string SubfolderWebApiModelsGeneratedKey = "Generated";

            /// <summary> BusinessRepositoryKey is used as a dictionary key for projects </summary>
            public const string BusinessRepositoryKey = "BusinessRepository";

            /// <summary> InterfacesKey is used as a dictionary key for projects </summary>
            public const string InterfacesKey = "Interfaces";

            /// <summary> ModelsKey is used as a dictionary key for projects </summary>
            public const string ModelsKey = "Models";

            /// <summary> ResourcesKey is used as a dictionary key for projects </summary>
            public const string ResourcesKey = "Resources";

            /// <summary> ServicesKey is used as a dictionary key for projects </summary>
            public const string ServicesKey = "Services";

            /// <summary> WebKey is used as a dictionary key for projects </summary>
            public const string WebKey = "Web";

            /// <summary> SubFolderModelKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderModelKey = "Model";

            /// <summary> SubFolderModelFieldsKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderModelFieldsKey = "ModelFields";

            /// <summary> SubFolderModelEnumsKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderModelEnumsKey = "ModelEnums";

            /// <summary> SubFolderBusinessRepositoryKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderBusinessRepositoryKey = "BusinessRepository";

            /// <summary> SubFolderBusinessRepositoryMappersKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderBusinessRepositoryMappersKey = "Mappers";

            /// <summary> SubFolderBusinessRepositoryMappersKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderInterfacesBusinessRepositoryKey = "BusinessRepository";

            /// <summary> SubFolderInterfacesServicesKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderInterfacesServicesKey = "Services";

            /// <summary> SubFolderServicesKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderServicesKey = "Services";

            /// <summary> SubFolderUnitOfWorkKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderUnitOfWorkKey = "UnitOfWork";

            /// <summary> SubFolderResourcesKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderResourcesKey = "Resources";

            /// <summary> SubFolderWebIndexKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderWebIndexKey = "Index";

            /// <summary> SubFolderWebLocalizationKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderWebLocalizationKey = "Localization";

            /// <summary> SubFolderWebViewModelKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderWebViewModelKey = "ViewModel";

            /// <summary> SubFolderWebControllersKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderWebControllersKey = "Controllers";

            /// <summary> SubFolderWebFinderKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderWebFinderKey = "Finder";

            /// <summary> SubFolderWebScriptsKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderWebScriptsKey = "Scripts";

            /// <summary> SubFolderWebFinderDefKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderWebFinderDefKey = "FinderDef";

            /// <summary> SubFolderWebSqlKey is used as a dictionary key for subfolders </summary>
            public const string SubFolderWebSqlKey = "Sql";

            /// <summary> SubFolderNameFields is used as a subfolder name </summary>
            public const string SubFolderNameFields = "Fields";

            /// <summary> SubFolderWebApiControllers is used as a subfolder name </summary>
            public const string SubFolderWebApiControllers = "Controllers";

            /// <summary> SubFolderWebApiVersioning is used as a subfolder name </summary>
            public const string SubFolderWebApiVersioning = "Versioning";

            /// <summary> SubFolderWebApiModelsCustom is used as a subfolder name </summary>
            public const string SubFolderWebApiModelsCustom = "Custom";

            /// <summary> SubFolderWebApiModelsGenerated is used as a subfolder name </summary>
            public const string SubFolderWebApiModelsGenerated = "Generated";


            /// <summary> SubFolderNameEnums is used as a subfolder name </summary>
            public const string SubFolderNameEnums = "Enums";

            /// <summary> SubFolderNameMappers is used as a subfolder name </summary>
            public const string SubFolderNameMappers = "Mappers";

            /// <summary> SubFolderNameBusinessRepository is used as a subfolder name </summary>
            public const string SubFolderNameBusinessRepository = "BusinessRepository";

            /// <summary> SubFolderNameServices is used as a subfolder name </summary>
            public const string SubFolderNameServices = "Services";

            /// <summary> SubFolderNameForms is used as a subfolder name </summary>
            public const string SubFolderNameForms = "Forms";

            /// <summary> SubFolderNameProcess is used as a subfolder name </summary>
            public const string SubFolderNameProcess = "Process";

            /// <summary> SubFolderNameReports is used as a subfolder name </summary>
            public const string SubFolderNameReports = "Reports";

            /// <summary> SubFolderNameUnitOfWork is used as a subfolder name </summary>
            public const string SubFolderNameUnitOfWork = "UnitOfWork";

            /// <summary> SubFolderNameAreas is used as a subfolder name </summary>
            public const string SubFolderNameAreas = "Areas";

            /// <summary> SubFolderNameViews is used as a subfolder name </summary>
            public const string SubFolderNameViews = "Views";

            /// <summary> SubFolderNamePartials is used as a subfolder name </summary>
            public const string SubFolderNamePartials = "Partials";

            /// <summary> SubFolderNameModels is used as a subfolder name </summary>
            public const string SubFolderNameModels = "Models";

            /// <summary> SubFolderNameControllers is used as a subfolder name </summary>
            public const string SubFolderNameControllers = "Controllers";

            /// <summary> SubFolderNameFinder is used as a subfolder name </summary>
            public const string SubFolderNameFinder = "Finder";

            /// <summary> SubFolderNameScripts is used as a subfolder name </summary>
            public const string SubFolderNameScripts = "Scripts";

            /// <summary> Element Name for Entities </summary>
            public const string ElementEntities = "entities";

            /// <summary> New Entity Text </summary>
            public const string NewEntityText = "new";

            /// <summary> Property for Module </summary>
            public const string PropertyModule = "module";

            /// <summary> Property for ViewId </summary>
            public const string PropertyViewId = "view";

            /// <summary> Property for Entity </summary>
            public const string PropertyEntity = "entity";

            /// <summary> Property for Entity </summary>
            public const string PropertyModel = "model";

            /// <summary> Property for Include </summary>
            public const string PropertyInclude = "include";

            /// <summary> Property for Container </summary>
            public const string PropertyContainer = "container";

            /// <summary> Property for Properties </summary>
            public const string PropertyProperties = "props";

            /// <summary> Property for Compositions </summary>
            public const string PropertyComps = "comps";

            /// <summary> Property for Compositions </summary>
            public const string PropertyCompositions = "compositions";

            /// <summary> Property for Composition </summary>
            public const string PropertyComposition = "composition";

            /// <summary> Property for Type </summary>
            public const string PropertyType = "type";

            /// <summary> Property for Program </summary>
            public const string PropertyProgramId = "program";

            /// <summary> Property for WorkflowId </summary>
            public const string PropertyWorkflowId = "workflow";

            /// <summary> Property for Report </summary>
            public const string PropertyReport = "report";

            /// <summary> Property for Finder </summary>
            public const string PropertyFinder = "finder";

            /// <summary> Property for Grid </summary>
            public const string PropertyGrid = "grid";

            /// <summary> Property for Grid </summary>
            public const string PropertyGridModel = "gridmodel";

            /// <summary> Property for Grid </summary>
            public const string PropertySequenceRevisionList = "sequencerevisionlist";

            /// <summary> Property for Enablement </summary>
            public const string PropertyEnablement = "enablement";

            /// <summary> Property for ClientFiles </summary>
            public const string PropertyClientFiles = "clientFiles";

            /// <summary> Property for IfExists </summary>
            public const string PropertyIfExists = "ifExists";

            /// <summary> Property for SingleFile </summary>
            public const string PropertySingleFile = "singleFile";

            /// <summary> Property for Fields </summary>
            public const string PropertyFields = "fields";

            /// <summary> Property for Field </summary>
            public const string PropertyField = "field";

            /// <summary> Property for FieldName </summary>
            public const string PropertyFieldName = "fieldName";

            /// <summary> Property for Key Field </summary>
            public const string PropertyKeyField = "KeyField";


            /// <summary> Property for PropertyName </summary>
            public const string PropertyPropertyName = "propertyName";

            /// <summary> Property for Size </summary>
            public const string PropertySize = "size";

#if ENABLE_TK_244885
            /// <summary> Property for IsCommon </summary>
            public const string PropertyIsCommon = "common";

            ///// <summary> Property for AlternateName </summary>
            //public const string PropertyAlternateName = "alternateName";
#endif

            /// <summary> Property for ResxName </summary>
            public const string PropertyResxName = "resx";

            /// <summary> Property for Options </summary>
            public const string PropertyOptions = "options";

            /// <summary> Property for Option </summary>
            public const string PropertyOption = "option";

            /// <summary> Constant for EntityName </summary>
            public const string ConstantEntityName = "EntityName";

            public const string FinderDefFileName = "FinderDef.js";
        }
#endregion

#region Public Delegates

        /// <summary> Delegate to update UI with name of file being processed </summary>
        /// <param name="text">Text for UI</param>
        public delegate void ProcessingEventHandler(string text);

        /// <summary> Delegate to update UI with status of file being processed </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="statusType">Status Type</param>
        /// <param name="text">Text for UI</param>
        public delegate void StatusEventHandler(string fileName, Info.StatusTypeEnum statusType, string text);

#endregion

#region Public Events

        /// <summary> Event to update UI with name of file being processed </summary>
        public event ProcessingEventHandler ProcessingEvent;

        /// <summary> Event to update UI with status of file being processed </summary>
        public event StatusEventHandler StatusEvent;

#endregion

#region Constructor

#endregion

#region Public Methods

        /// <summary> Cleanup </summary>
        public void Dispose()
        {
            // Anything?
        }

        /// <summary> Start the generation process for Web project </summary>
        /// <param name="settings">Settings for processing</param>
        public void ProcessWeb(Settings settings)
        {
            try
            {
                _settings = settings;

                if (_settings.RepositoryType.Equals(RepositoryType.HeaderDetail))
                {
                    // Find out the header view and use this view to pass view related information
                    // around to satisfy the parameter requirements for the existing routines.
                    var headerView = settings.Entities.First(e => e.Properties[BusinessView.Constants.ViewId] == settings.HeaderNode.Attribute(Constants.PropertyViewId).Value);

                    // Build the subfolders
                    BuildSubfolders(headerView);

                    CreateHeaderDetailRepositoryClasses(headerView, _settings);

                }

                // Iterate Views
                foreach (var businessView in settings.Entities)
                {
                    // Special logic for header detail type
                    if (_settings.RepositoryType.Equals(RepositoryType.HeaderDetail))
                    {
                        var entityName = businessView.Properties[BusinessView.Constants.EntityName];

                        // Create the Model Fields class
                        CreateClass(businessView,
                                entityName + "Fields.cs",
                                TransformTemplateToText(businessView, _settings, "Templates.Common.Class.ModelFields"),
                                Constants.ModelsKey, Constants.SubFolderModelFieldsKey);

                        // If generate model is true, create class for model, resource, and enums (if any)
                        // Note: This is required for a detail class that has a grid AND
                        // will display a popup screen based on the grid since the razor view
                        // for the popup will require a model for html helpers
                        if (businessView.Options[BusinessView.Constants.GenerateGridModel])
                        {
                            // Create the Model class
                            CreateClass(businessView,
                                        entityName + ".cs",
                                        TransformTemplateToText(businessView, _settings, "Templates.Common.Class.Model"),
                                        Constants.ModelsKey, Constants.SubFolderModelKey);
                            
                            // Create the Resx Files if the MenuResx file for that language exists
                            CreateResxFilesByLanguage(_settings, businessView);

                            // Create enums, if any
                            foreach (var value in businessView.Enums.Values)
                            {
                                _settings.EnumHelper = value;

                                CreateClass(businessView,
                                    value.Name + ".cs",
                                    TransformTemplateToText(businessView, _settings, "Templates.Common.Class.ModelEnums"),
                                    Constants.ModelsKey, Constants.SubFolderModelEnumsKey);
                            }
                        }

                        // @JT research this  --> continue;
                    }

                    IterateView(businessView);
                    UpdateModel(businessView);
                }

                 //Generate Json configuration files for grid
                 GenerateGridJsonFiles();
            }
            catch (Exception e)
            {
                var message = e.Message;

                // Catch here does nothing but return to UI
                // User may have aborted wizard
            }
        }

        /// <summary> Generate Web API Models </summary>
        /// <param name="settings">Settings object</param>
        /// <param name="ControllerSettings">Controller Settings</param>
        private void GenerateWebApiModelsForAllEntities(Settings settings, List<ControllerSettings> ControllerSettings)
        {
            foreach(var controllerSettings in ControllerSettings)
            {
                CreateClass(controllerSettings.BusinessView, controllerSettings.BusinessView.Text + "Generated.cs",
                    WebApiTransformTemplateToText(settings, controllerSettings,
                        "Templates.WebApi.ModelGenerated"),
                    Constants.WebApiModelsKey, Constants.SubfolderWebApiModelsGeneratedKey);

                CreateClass(controllerSettings.BusinessView, controllerSettings.BusinessView.Text + ".cs",
                    WebApiTransformTemplateToText(settings, controllerSettings,
                        "Templates.WebApi.ModelCustom"),
                    Constants.WebApiModelsKey, Constants.SubfolderWebApiModelsCustomKey);

                GenerateWebApiModelsForAllEntities(settings, controllerSettings.Details);
            }
        }

        /// <summary> Process Web Api project </summary>
        /// <param name="settings">Settings object</param>
        public void ProcessWebApi(Settings settings)
        {
            // Locals
            var session = new Session();
            try
            {
                _settings = settings;

                BuildWebApiSubfolders();

                foreach (var controllerSettings in settings.ControllerSettings)
                {

                    CreateClass(controllerSettings.BusinessView, controllerSettings.BusinessView.Properties[BusinessView.Constants.ModuleId] + controllerSettings.BusinessView.Properties[BusinessView.Constants.ResourceName] + "Controller.cs",
                        WebApiTransformTemplateToText(settings, controllerSettings,
                            "Templates.WebApi.Controller"),
                        Constants.WebApiKey, Constants.SubfolderWebApiControllerKey);

                    UpdateWebApiVersioning(controllerSettings.BusinessView, WebApiTransformTemplateToText(settings, controllerSettings,
                        "Templates.WebApi.VersionController"));
                }

                GenerateWebApiModelsForAllEntities(settings, settings.ControllerSettings);
            }
            catch
            {
                // Seems like not all views have an instance protocol (i.e., AS0020)
            }

        }


        /// <summary> Get the program Id from desktop SDK(i.e. ARCUS for AR0024) </summary>
        /// <param name="dbLink">DB Link Object</param>
        /// <param name="viewId">View Id</param>
        /// <returns>Program Id</returns>
        /// <remarks>Logic copied from .NET API</remarks>
        private static string GetProgramId(DBLink dbLink, string viewId )
        {
            // SDK Roto ID to Query
            var rotoIdView = dbLink.OpenView(XXROTO.ROTOID);

            rotoIdView.Fields.FieldByID(XXROTO.IDX_OPERATION).SetValue(XXROTO.OPERATION_GetRotoList, false);
            rotoIdView.Fields.FieldByID(XXROTO.IDX_ROTOTYPE).SetValue(XXROTO.ROTOTYPE_View, false);
            rotoIdView.Fields.FieldByID(XXROTO.IDX_PGMID).SetValue(viewId.Substring(0,2), false);
            rotoIdView.Process();

            var views = rotoIdView.Fields.FieldByID(XXROTO.IDX_OBJECTID).PresentationList;
            var viewPaths = rotoIdView.Fields.FieldByID(XXROTO.IDX_OBJECTNAME).PresentationList;

            var programId = string.Empty;
            for (var i = 0; i < views.Count; i++)
            {
                if (views.PredefinedString(i).ToUpper().Trim() == viewId.ToUpper().Trim())
                {
                    programId = Path.GetFileNameWithoutExtension(viewPaths.PredefinedString(i).ToUpper()).Trim();
                    break;
                }
            }
            rotoIdView.Dispose();
            return programId;
        }

        /// <summary> Get business view </summary>
        /// <param name="businessView">Business View</param>
        /// <param name="user">User Name</param>
        /// <param name="password">User Password</param>
        /// <param name="company">Company</param>
        /// <param name="version">Version</param>
        /// <param name="viewId">View Id</param>
        /// <param name="moduleId">Module Id</param>
        /// <param name="wizardType">Web or Web API</param>
        public static void GetBusinessView(BusinessView businessView, string user,
            string password, string company, string version, string viewId, string moduleId, WizardType wizardType)
        {
            // Locals
            var session = new Session();
            var uniqueDescriptions = new Dictionary<string, bool>();

            session.InitEx2(null, string.Empty, "WX", "WX1000", version, 1);
            session.Open(user, password, company, DateTime.UtcNow, 0);

            // Attempt to open a view
            var dbLink = session.OpenDBLink(DBLinkType.Company, DBLinkFlags.ReadOnly);
            var view = dbLink.OpenView(viewId);

            // Clear out business view except for text
            businessView.Enums.Clear();
            businessView.Fields.Clear();
            //businessView.Keys.Clear();
            businessView.Options.Clear();
            businessView.Properties.Clear();
            businessView.Compositions.Clear();
            try
            {
                businessView.Protocol = view.InstanceProtcol;
            }
            catch
            {
                // Seems like not all views have an instance protocol (i.e., AS0020)
            }

            businessView.Properties[BusinessView.Constants.ViewId] = view.ViewID;
            businessView.Properties[BusinessView.Constants.ModuleId] = moduleId;

            GenerateUniqueDescriptions(view, uniqueDescriptions);
            
            var description = MakeItSingular(BusinessViewHelper.Replace(view.Description));

            businessView.Properties[BusinessView.Constants.ModelName] = description;
            businessView.Properties[BusinessView.Constants.EntityName] = description;
            businessView.Properties[BusinessView.Constants.ResourceName] = BusinessViewHelper.Replace(view.Description);
            businessView.Properties[BusinessView.Constants.PropertyName] = businessView.Properties[BusinessView.Constants.ResourceName];
            businessView.Properties[BusinessView.Constants.ProgramId] = wizardType == WizardType.WEBAPI ? GetProgramId(dbLink, view.ViewID) : string.Empty;

            businessView.PrimaryKeyFields = new List<string>();
            if (view.Keys.Count > 0)
            {
                var key = view.Keys[0];
                for (int i=0; i < key.FieldCount; i++)
                {
                    businessView.PrimaryKeyFields.Add(key.Field(i).Name);
                }
            }

#if ENABLE_TK_244885
            businessView.Properties[BusinessView.Constants.CustomCommonResxName] = PrivateConstants.CustomCommonResx;
#endif
            GenerateFieldsAndEnums(businessView, view, uniqueDescriptions, wizardType);

            // Any compositions
            if (view.CompositeNames != null)
            {
                if (view.CompositeNames.Count() > 0)
                {
                    foreach (var compositeView in view.CompositeNames)
                    {
                        businessView.Compositions.Add(new Composition() { ViewId = compositeView });
                    }
                }
            }

            // Clean up
            try
            {
                view.Dispose();
                dbLink.Dispose();
                session.Dispose();
            }
            catch
            {
                // Swallow error, if any        
            }

        }

        #endregion

        #region Private Methods        
        /// <summary> ReplacePlaceHolder </summary>
        /// <param name="nameList">List of names</param>
        /// <param name="valueList">List of values</param>
        /// <param name="content">Replacement content</param>
        /// <returns>content</returns>
        private string ReplacePlaceHolder(string[] nameList, string[] valueList, string content)
        {
            var length = nameList.Length;
            for (int i = 0; i < length; i++)
            {
                content = content.Replace(nameList[i], valueList[i]);
            }
            return content;
        }

        /// <summary> Update model definition to add optional field </summary>
        /// <param name="entity"></param>
        private void UpdateModel(BusinessView entity)
        {
            var module = entity.Properties[BusinessView.Constants.ModuleId];
            var projectInfo = _settings.Projects[Constants.ModelsKey][module];
            var modelFile = Path.Combine(projectInfo.ProjectFolder, entity.Text + ".cs");
            if (File.Exists(modelFile))
            {
                var fs = File.OpenText(modelFile);
                var sb = new StringBuilder();
                string line;

                while ((line = fs.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                    if (line.Contains("int NumberOfOptionalFields"))
                    {
                        sb.Append(CodeSnippet.HASOptionalFieldDef);
                    }
                }
                fs.Close();
                File.WriteAllText(modelFile, sb.ToString());
            }
        }

        /// <summary> Update the version file </summary>
        /// <param name="entity"></param>
        /// <param name="versioning"></param>
        private void UpdateWebApiVersioning(BusinessView entity, string versioning)
        {
            var module = entity.Properties[BusinessView.Constants.ModuleId];
            var projectInfo = _settings.Projects[Constants.WebApiKey][module];

            var subFolder = projectInfo.Subfolders[Constants.SubfolderWebApiVersioningKey];
            var filePath = BusinessViewHelper.ConcatStrings(new[] { projectInfo.ProjectFolder, subFolder });
            var fullFileName = BusinessViewHelper.ConcatStrings(new[] { filePath, module + "ControllerVersionMapGenerated.cs" });
            if (File.Exists(fullFileName))
            {
                var fs = File.OpenText(fullFileName);
                var sb = new StringBuilder();
                string line;

                while ((line = fs.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                    if (line.Trim().Equals("{"))
                    {
                        sb.Append(versioning);
                    }
                }
                fs.Close();
                File.WriteAllText(fullFileName, sb.ToString());
            }
        }

        /// <summary> Lower the string first char </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private string LowerFirstChar(string inputString) 
        {
            return inputString.Substring(0, 1).ToLower() + inputString.Substring(1);
        }

        /// <summary> Get names from entities for replace snippet code placeholders </summary>
        /// <returns></returns>
        private IDictionary<string, string> GetNameFromEntity()
        {
            string[] names = { "", "", "", "", "" };
            IDictionary<string, string> dicNames = new Dictionary<string, string>();
            var modelName = LowerFirstChar(_settings.Entities[0].Text);

            dicNames.Add("MODELNAME", modelName);
            dicNames.Add("UIOBJECTNAME", modelName + "UI");

            if (_settings.RepositoryType.Equals(RepositoryType.HeaderDetail) && _settings.Entities.Count >= 4)
            {
                var detailEntity = _settings.Entities.First(e => e.ForGrid && !e.Keys.Contains("OptionalField"));
                var fields = detailEntity.Fields;
                var optionalFieldEntity = _settings.Entities.First(e => e.Keys.Contains("OptionalField") && e.Keys.Count == 2);
                var detailOptionalFieldEntity = _settings.Entities.First(e => e.Keys.Contains("OptionalField") && e.Keys.Count == 3);

                dicNames.Add("DGRIDNAME", LowerFirstChar(detailEntity.Text) + "Grid");
                dicNames.Add("OFGRIDNAME", LowerFirstChar(optionalFieldEntity.Text) + "Grid");
                dicNames.Add("OFDGRIDNAME", LowerFirstChar(detailOptionalFieldEntity.Text) + "Grid");

                dicNames.Add("SEQ", fields[0].ServerFieldName);
                dicNames.Add("LINENO", fields[1].ServerFieldName);
            }

            if (_settings.RepositoryType.Equals(RepositoryType.Flat) && _settings.Entities.Count(e=>e.ForGrid) >= 2) 
            {
                var header = _settings.Entities[0];
                var detail = _settings.Entities[1];
                var detailKey = detail.Fields[0].ServerFieldName;
                dicNames.Add("HeaderGrid", LowerFirstChar(header.Text) + "Grid");
                dicNames.Add("DetailGrid", LowerFirstChar(detail.Text) + "Grid");
                dicNames.Add("DetailGridKey", detailKey);
            }

            return dicNames;
        }
        /// <summary> Use Code Snippet to Update JavaScript Code </summary>
        private void UpdateJavaSciptCode(BusinessView entity, List<BusinessView> gridEntities, List<string> displayColumns, List<string> titles, List<string> colTypes, List<string> urls)
        {
            //var gridSnippet = CodeSnippet.JSReadOnlyGridDef;
            var isHeaderDetail = _settings.RepositoryType.Equals(RepositoryType.HeaderDetail);
            var name = isHeaderDetail ? _settings.EntitiesContainerName : entity.Text;
            var module = entity.Properties[BusinessView.Constants.ModuleId];
            var projectInfo = _settings.Projects[Constants.WebKey][module];
            var jsPath = Path.Combine(projectInfo.ProjectFolder, string.Format(@"Areas\{0}\Scripts\{1}", module, name));
            var fileName = projectInfo.ProjectName.Replace("Web", name + "Behaviour.js");
            var jsFilePath = Path.Combine(jsPath, fileName);
            var names = GetNameFromEntity();

            if (File.Exists(jsFilePath))
            {
                var fs = File.OpenText(jsFilePath);
                var sb = new StringBuilder();
                string line;

                while ((line = fs.ReadLine()) != null)
                {
                    if (line.Contains("initTimePickers: function () {") && isHeaderDetail)
                    {
                        var nameList = new string[] { "{OFGRIDNAME}", "{OFDGRIDNAME}", "{DGRIDNAME}", "{SEQ}", "{LINENO}" };
                        var valueList = new string[] { names["OFGRIDNAME"], names["OFDGRIDNAME"], names["DGRIDNAME"], names["SEQ"], names["LINENO"] };
                        var codeSnippet = ReplacePlaceHolder(nameList, valueList, CodeSnippet.JSShowOptionalFieldDef);
                        sb.Append(codeSnippet);
                    }

                    if (line.Contains("initialLoad: function (result) {"))
                    {
                        if (names.ContainsKey("HeaderGrid"))
                        {
                            var nameList = new string[] { "{HeaderGrid}", "{DetailGrid}", "{DetailGridKey}", "{EntityName}" };
                            var valueList = new string[] { names["HeaderGrid"], names["DetailGrid"], names["DetailGridKey"], LowerFirstChar(entity.Text) };
                            var codeSnippet = ReplacePlaceHolder(nameList, valueList, CodeSnippet.JSSyncHeaderDetailGrid);
                            sb.Append(codeSnippet);
                        }
                        sb.Append(CodeSnippet.JSCustomFunctionsDef.Replace("{EntityName}", LowerFirstChar(entity.Text)));
                    }

                    sb.AppendLine(line);

                    if (line.Contains("initPopUps: function () {") && isHeaderDetail)
                    {
                        var nameList = new string[] { "{OFGRIDNAME}", "{OFDGRIDNAME}", "{DGRIDNAME}" };
                        var valueList = new string[] { names["OFGRIDNAME"], names["OFDGRIDNAME"], names["DGRIDNAME"] };
                        var codeSnippet = ReplacePlaceHolder(nameList, valueList, CodeSnippet.JSOptionalFieldPopUpDef);
                        sb.Append(codeSnippet);
                    }
                    if (line.Contains("initHamburgers: function () {") && isHeaderDetail)
                    {
                        var codeSnippet = CodeSnippet.JSOptionalFieldHamburgerDef.Replace("{MODELNAME}", names["MODELNAME"]);
                        sb.Append(codeSnippet);
                    }
                }
                fs.Close();
                File.WriteAllText(jsFilePath, sb.ToString());
            }
        }

        /// <summary> Generate Grid Json configuration file </summary>
        private void GenerateGridJsonFiles()
        {
            var layout = _settings.XmlLayout;
            if (layout == null)
            {
                return;
            }

            var entities = new List<BusinessView>();
            var displayColumns = new List<string>();
            var titles = new List<string>();
            var colTypes = new List<string>();
            var colUrls = new List<string>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(layout.Root.ToString());
            var gridNodes = xmlDoc.SelectNodes("//Control[@widget='Grid']");
            var keyNode = xmlDoc.SelectSingleNode("//Control[@widget='Finder']");
            var keyEntityName = keyNode != null ?  keyNode.Attributes["entity"].Value : _settings.Entities[0].Text;
            var isHeadDetail = _settings.RepositoryType.Equals(RepositoryType.HeaderDetail);
            var isFlat = _settings.RepositoryType.Equals(RepositoryType.Flat);

            if (gridNodes.Count > 0)
            {
                var index = 0;
                foreach (XmlNode node in gridNodes)
                {
                    var hasColumns = node.ChildNodes[0].ChildNodes.Count > 0;
                    // Generate Grid Json configuration file in View Folder
                    if (hasColumns)
                    {
                        var jsonObj = new GridDefinition();
                        jsonObj.GridType = 0;
                        jsonObj.ReadOnly = false;
                        jsonObj.ViewOrder = 0;
                        jsonObj.PageSize = 10;

                        if (isFlat && gridNodes.Count >= 2 && index == 0)
                        {
                            var gridCustomFunctions = new List<GridCustomFunction>();
                            var func1 = new GridCustomFunction();
                            func1.gridAfterLoadData = LowerFirstChar(keyEntityName) + "UISuccess.customGridAfterLoadData";
                            gridCustomFunctions.Add(func1);
                            var func2 = new GridCustomFunction();
                            func2.gridAfterSetActiveRecord = LowerFirstChar(keyEntityName) + "UISuccess.customGridAfterSetActiveRecord";
                            gridCustomFunctions.Add(func2);
                            jsonObj.CustomFunctions = gridCustomFunctions;
                        }

                        index++;
                        var cols = new List<ColumnDefinition>();
                        XmlNodeList nodeList = node.ChildNodes[0].ChildNodes;
                        BusinessView entity = null;
                        var colIndex = 0;
                        foreach (XmlNode colNode in nodeList)
                        {
                            var col = new ColumnDefinition();
                            var finderProp = colNode.Attributes["finderProperty"]?.Value;
                            var entityName = colNode.Attributes["entity"].Value;
                            var propName = colNode.Attributes["property"].Value;
                            entity = _settings.Entities.First(e => e.Properties[BusinessView.Constants.EntityName] == entityName);
                            var fields = entity.Fields;
                            var field = fields.First(f => f.Name == propName);
                            var isLineNo = field.ServerFieldName == "LINENO";
                            var isLookupFinder = finderProp != null;

                            col.ColumnName = field.Description;
                            col.FieldName = field.ServerFieldName;
                            col.DataType = field.Type.ToString();
                            col.IsEditable = (field.IsKey || field.IsCalculated || field.IsReadOnly || isLineNo) && isHeadDetail ? false : true;
                            if (isHeadDetail && isLineNo)
                            {
                                col.IsLineNumber = true;
                            }
                            if (entity.Keys.Contains(propName) || isLookupFinder)
                            {
                                col.HasFinder = true;
                                var finder = new FinderDefinition();

                                dynamic lookupFinder = null;
                                if (isLookupFinder)
                                {
                                    lookupFinder = _settings.FinderInfo[finderProp];
                                }

                                // Logic specific to PR
                                var module_Id = entity.Properties[BusinessView.Constants.ModuleId];
                                string view_Id = isLookupFinder ? lookupFinder.viewID : entity.Properties[BusinessView.Constants.ViewId];
                                if (module_Id == "PR")
                                {
                                    if (view_Id.StartsWith("UP") || view_Id.StartsWith("CP"))
                                    {
                                        view_Id = "~~" + view_Id.Substring(2);
                                    }
                                }
                                finder.ViewID = view_Id;

                                finder.ViewOrder = isLookupFinder ? Int32.Parse(lookupFinder.viewOrder.ToString()) : 0;
                                finder.DisplayFieldNames = isLookupFinder ? lookupFinder.displayFieldNames :  entity.Fields.Select(f => f.ServerFieldName).Take(8).ToArray();
                                finder.ReturnFieldNames = isLookupFinder ? lookupFinder.returnFieldNames : new string[] { entity.Fields[0].ServerFieldName };
                                finder.Filter = "";
                                finder.InitKeyFieldNames =new string[] { };
                                col.Finder = finder;
                            } else {
                                col.HasFinder = null;
                            }

                            if (isFlat && colIndex < 2)
                            {
                                if (col.CustomFunctions == null)
                                {
                                    col.CustomFunctions = new List<CustomFunction>();
                                }
                                
                                if (colIndex == 0)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        var customFunction = new CustomFunction();
                                        if (i== 0)
                                        {
                                            customFunction.columnBeforeFinder = LowerFirstChar(keyEntityName) + "UISuccess.changeFinderDefinition";
                                        }
                                        else
                                        {
                                            customFunction.columnFinderFocus = LowerFirstChar(keyEntityName) + "UISuccess.showHideFinderButton";
                                        }
                                        col.CustomFunctions.Add(customFunction);
                                    }
                                }
                                else 
                                {
                                    var customFunction = new CustomFunction();
                                    customFunction.columnBeforeDisplay = LowerFirstChar(keyEntityName) + "UISuccess.showPencilIcon";
                                    col.CustomFunctions.Add(customFunction);
                                }
                            }
                            cols.Add(col);
                            colIndex++;
                        }

                        var optionalField = entity.Fields.FirstOrDefault(f => f.Name == "HASOPT");
                        if (isHeadDetail && optionalField != null) 
                        {
                            var col = new ColumnDefinition();
                            var customFunction = new CustomFunction();
                            customFunction.OptionalField = "{UIOBJECTNAME}.showDetailOptionalField";
                            col.ColumnName = optionalField.Description;
                            col.FieldName = "HASOPT";
                            col.IsOptionalField = true;
                            col.CustomFunctions = new List<CustomFunction>();
                            col.CustomFunctions.Add(customFunction);
                            cols.Add(col);
                        }

                        jsonObj.ColumnDefinitions = cols;

                        // Logic specific to PR
                        var moduleId = entity.Properties[BusinessView.Constants.ModuleId];
                        string viewId = entity.Properties[BusinessView.Constants.ViewId];
                        if (moduleId == "PR")
                        {
                            if (viewId.StartsWith("UP") || viewId.StartsWith("CP"))
                            {
                                viewId = "~~" + viewId.Substring(2);
                            }
                        }
                        jsonObj.ViewID = viewId;

                        entities.Add(entity);
                        displayColumns.Add("'" + string.Join("','", cols.Select(c => c.FieldName)) + "'");
                        titles.Add("'" + string.Join("','", cols.Select(c => c.ColumnName)) + "'");
                        colTypes.Add("'" + string.Join("','", cols.Select(c => c.DataType)) + "'");

                        var urls = new List<string>();
                        foreach (var col in cols)
                        {
                            var url = string.Empty;
                            if (entity.Keys.Contains(col.ColumnName.Replace(" ", "")))
                            {
                                var module = entity.Properties[BusinessView.Constants.ModuleId];
                                var entityName = entity.Properties[BusinessView.Constants.EntityName];
                                url = string.Format("{0}/{1}/Index", module, keyEntityName);
                            }
                            urls.Add(string.Format("{0}", url));
                        }
                        colUrls.Add("'" + string.Join("','", urls) + "'");

                        var fileName = entity.Properties[BusinessView.Constants.EntityName] + "Grid.json";
                        var jsonString = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        var isHeaderDetail = _settings.RepositoryType.Equals(RepositoryType.HeaderDetail);

                        if (isHeaderDetail)
                        {
                            var uiObjectName =  LowerFirstChar(_settings.Entities[0].Text) + "UI";
                            jsonString = jsonString.Replace("{UIOBJECTNAME}", uiObjectName);
                        }
                        entity = _settings.Entities.FirstOrDefault(e => e.Properties[BusinessView.Constants.EntityName] == keyEntityName);
                        if (entity == null)
                        {
                            entity = _settings.Entities[0];
                        }
                        CreateClass(entity, fileName, jsonString, Constants.WebKey, Constants.SubFolderWebLocalizationKey, true, !_settings.RepositoryType.Equals(RepositoryType.HeaderDetail));
                    }
                }
                var keyEntity = _settings.Entities.FirstOrDefault(e => e.Properties[BusinessView.Constants.EntityName] == keyEntityName);
                UpdateJavaSciptCode(keyEntity, entities, displayColumns, titles, colTypes, colUrls);
            }
        }

        /// <summary> Save a file </summary>
        /// <param name="view">Business View</param>
        /// <param name="fileName"> Name of file to be created</param>
        /// <param name="content">File contents</param>
        /// <param name="projectKey">Project Key for Project Info</param>
        /// <param name="subfolderKey">Subfolder Key for Project Info</param>
        /// <param name="addToProject">True to add to project otherwise false</param>
        /// <returns>True if successful otherwise false</returns>
        private bool SaveFile(BusinessView view, string fileName, string content, string projectKey,
            string subfolderKey, bool addToProject, bool forGrid)
        {
            // Local
            var retVal = true;
            var module = view.Properties[BusinessView.Constants.ModuleId];
            var projectInfo = _settings.Projects[projectKey][module];
            var subFolder = forGrid ? string.Format(@"Areas\{0}\Views\{1}\Partials", module, view.Text ) : projectInfo.Subfolders[subfolderKey];
            var filePath = BusinessViewHelper.ConcatStrings(new[] { projectInfo.ProjectFolder, subFolder });
            var fullFileName = BusinessViewHelper.ConcatStrings(new[] { filePath, fileName });

            // Determine if exists
            var exists = File.Exists(fullFileName);
            var writeFile = true;

            // Prompt if file exists?
            if (exists && _settings.PromptIfExists)
            {
                var result = MessageBox.Show(string.Format(Resources.FileExists, fullFileName),
                                             Resources.Confirmation, 
                                             MessageBoxButtons.YesNoCancel, 
                                             MessageBoxIcon.Question);

                // Evaluate
                switch (result)
                {
                    case DialogResult.Yes:
                        // Overwrite the file
                        break;
                    case DialogResult.No:
                        // Skip the file
                        writeFile = false;
                        break;
                    case DialogResult.Cancel:
                        // Abort the wizard
                        throw new Exception();
                }
            }

            // Write the file?
            if (writeFile)
            {
                try
                {
                    // Ensure the filepath exists
                    if (!exists)
                    {
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                    }

                    // Save the file
                    File.WriteAllText(fullFileName, content);
                }
                catch
                {
                    retVal = false;
                }

                // Update project if write was successful
                if (retVal && addToProject)
                {
                    // May need multiple attempts as Visual Visual Studio has been reporting
                    // not ready status intermediately
                    var attempts = 0;
                    var maxAttempts = 2;

                    while (attempts <= maxAttempts)
                    {
                        // Add to project
                        try
                        {
                            projectInfo.Project.ProjectItems.AddFromFile(fullFileName);
                            projectInfo.Project.Save(projectInfo.Project.FullName);
                            break;
                        }
                        catch
                        {
                            // Added a delay before retrying
                            Thread.Sleep(500);
                            attempts++;
                        }
                    }

                    // Determine if the save was a success
                    retVal = (attempts <= maxAttempts);
                }
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary> Build subfolders for later use by SaveFile routine </summary>
        /// <param name="view">Business View</param>
        private void BuildSubfolders(BusinessView view)
        {
            // Iterate Models
            var projects = _settings.Projects[Constants.ModelsKey];
            foreach (var project in projects)
            {
                var subfolders = new Dictionary<string, string>
                {
                    {Constants.SubFolderModelKey, GetSubfolderName(string.Empty)},
                    {Constants.SubFolderModelFieldsKey, GetSubfolderName(Constants.SubFolderNameFields)},
                    {Constants.SubFolderModelEnumsKey, GetSubfolderName(Constants.SubFolderNameEnums)}
                };
                project.Value.Subfolders = subfolders;
            }

            // Iterate BusinessRepository
            projects = _settings.Projects[Constants.BusinessRepositoryKey];
            foreach (var project in projects)
            {
                var subfolders = new Dictionary<string, string>
                {
                    {Constants.SubFolderBusinessRepositoryKey, GetSubfolderName(string.Empty)},
                    {Constants.SubFolderBusinessRepositoryMappersKey, GetSubfolderName(Constants.SubFolderNameMappers)}
                };
                project.Value.Subfolders = subfolders;
            }

            // Iterate Interfaces
            projects = _settings.Projects[Constants.InterfacesKey];
            foreach (var project in projects)
            {
                var subfolders = new Dictionary<string, string>
                {
                    {Constants.SubFolderInterfacesBusinessRepositoryKey, GetSubfolderName(Constants.SubFolderNameBusinessRepository)},
                    {Constants.SubFolderInterfacesServicesKey, GetSubfolderName(Constants.SubFolderNameServices)}
                };
                project.Value.Subfolders = subfolders;
            }

            // Iterate Services
            projects = _settings.Projects[Constants.ServicesKey];
            foreach (var project in projects)
            {
                var subfolders = new Dictionary<string, string>
                {
                    {Constants.SubFolderServicesKey, GetSubfolderName(string.Empty)},
                    {Constants.SubFolderUnitOfWorkKey, Constants.SubFolderNameUnitOfWork}
                };
                project.Value.Subfolders = subfolders;
            }

            // Iterate Resources
            projects = _settings.Projects[Constants.ResourcesKey];

            // Forms, Process and Reports for Resources are at the same level
            var subfolderName = Constants.SubFolderNameForms;
            switch (_settings.RepositoryType)
            {
                case RepositoryType.Process:
                    subfolderName = Constants.SubFolderNameProcess;
                    break;
                case RepositoryType.Report:
                    subfolderName = Constants.SubFolderNameReports;
                    break;
            }

            foreach (var project in projects)
            {
                var subfolders = new Dictionary<string, string>
                {
                    {Constants.SubFolderResourcesKey, subfolderName}
                };
                project.Value.Subfolders = subfolders;
            }

            // Iterate Web
            projects = _settings.Projects[Constants.WebKey];
            foreach (var project in projects)
            {
                // Need to determine web path first
                var moduleId = _settings.ModuleId;

                // Do web folders in "new" layout exist?
                var path = BusinessViewHelper.ConcatStrings(new[] { project.Value.ProjectFolder, Constants.SubFolderNameAreas, moduleId });

                if (Directory.Exists(path))
                {
                    path = BusinessViewHelper.ConcatStrings(new[] { Constants.SubFolderNameAreas, moduleId });
                }
                else
                {
                    path = project.Value.ProjectFolder;
                }

                var entityName = _settings.RepositoryType.Equals(RepositoryType.HeaderDetail) ? _settings.EntitiesContainerName : view.Properties[BusinessView.Constants.EntityName];

                var subfolders = new Dictionary<string, string>
                {
                    {Constants.SubFolderWebIndexKey, BusinessViewHelper.ConcatStrings(new []{path, Constants.SubFolderNameViews, entityName})},
                    {Constants.SubFolderWebLocalizationKey, BusinessViewHelper.ConcatStrings(new []{path, BusinessViewHelper.ConcatStrings(new []{ Constants.SubFolderNameViews, entityName, Constants.SubFolderNamePartials })})},
                    {Constants.SubFolderWebViewModelKey, GetSubfolderName(BusinessViewHelper.ConcatStrings(new []{path, Constants.SubFolderNameModels }))},
                    {Constants.SubFolderWebControllersKey, GetSubfolderName(BusinessViewHelper.ConcatStrings(new []{path, Constants.SubFolderNameControllers }))},
                    {Constants.SubFolderWebFinderKey, BusinessViewHelper.ConcatStrings(new []{path, Constants.SubFolderNameControllers, Constants.SubFolderNameFinder })},
                    {Constants.SubFolderWebSqlKey, BusinessViewHelper.ConcatStrings(new []{project.Value.ProjectFolder, string.Empty})},
                    {Constants.SubFolderWebScriptsKey, BusinessViewHelper.ConcatStrings(new []{path, Constants.SubFolderNameScripts, entityName})},
                    {Constants.SubFolderWebFinderDefKey, BusinessViewHelper.ConcatStrings(new []{path, Constants.SubFolderNameScripts})}
                };
                project.Value.Subfolders = subfolders;
            }
        }

        /// <summary> Build subfolders for later use by SaveFile routine </summary>
        private void BuildWebApiSubfolders()
        {
            // Iterate Models
            var projects = _settings.Projects[Constants.WebApiModelsKey];
            foreach (var project in projects)
            {
                var subfolders = new Dictionary<string, string>
                {
                    {Constants.SubfolderWebApiModelsCustomKey, GetSubfolderName(Constants.SubFolderWebApiModelsCustom)},
                    {Constants.SubfolderWebApiModelsGeneratedKey, GetSubfolderName(Constants.SubFolderWebApiModelsGenerated)},
                };
                project.Value.Subfolders = subfolders;
            }

            // Iterate BusinessRepository
            projects = _settings.Projects[Constants.WebApiKey];
            foreach (var project in projects)
            {
                var subfolders = new Dictionary<string, string>
                {
                    {Constants.SubfolderWebApiControllerKey, GetSubfolderName(Constants.SubFolderWebApiControllers)},
                    {Constants.SubfolderWebApiVersioningKey, GetSubfolderName(Constants.SubFolderWebApiVersioning)},
                };
                project.Value.Subfolders = subfolders;
            }
        }

        /// <summary> Get Sub Folder Name</summary>
        /// <param name="fixedName">Fixed portion of subfolder name, if any</param>
        /// <remarks>Will NOT have beginning or trailing slash</remarks>
        /// <returns>Subfolder for persistence</returns>
        private string GetSubfolderName(string fixedName)
        {
            // Locals
            var retVal = fixedName;

            // Is this a Process Type?
            retVal = retVal + ((_settings.RepositoryType.Equals(RepositoryType.Process)) ? @"\" + Constants.SubFolderNameProcess : string.Empty);

            // Is this a Report Type?
            retVal = retVal + ((_settings.RepositoryType.Equals(RepositoryType.Report)) ? @"\" + Constants.SubFolderNameReports : string.Empty);

            // Trim any begining slash
            if (retVal.StartsWith(@"\"))
            {
                retVal = retVal.Substring(1);
            }

            return retVal;
        }

        /// <summary> Make it singular or best guess at it! </summary>
        /// <param name="name">Name to make singular</param>
        /// <returns>Singular name or entered name if not plural</returns>
        public static string MakeItSingular(string name)
        {
            // Default to entered value
            var retVal = name;

            // If Options or ... then exit
            if (retVal.Equals("Options"))
            {
                return retVal;
            }

            // If it ends in an "s", make it singular
            if (retVal.EndsWith("s"))
            {
                retVal = retVal.Substring(0, retVal.Length - 1);
            }

            return retVal;

        }

        /// <summary> Get the Field Name. (Note: The Field description attribute is used for name) </summary>
        /// <param name="field">The field the name has to be determined</param>
        /// <param name="prefix">Prefix for certain potential values</param>
        /// <param name="uniqueDescriptions">Dictionary of unique descriptions</param>
        /// <returns>Name of the field</returns>
        private static string FieldName(ViewField field, string prefix, Dictionary<string, bool> uniqueDescriptions)
        {
            if (uniqueDescriptions != null &&
                uniqueDescriptions.ContainsKey(field.Description) &&
                uniqueDescriptions[field.Description] == false)
            {
                return field.Name;
            }

            var name = BusinessViewHelper.Replace(field.Description);
            return name.Equals("Type")? prefix + name : name;
        }


        /// <summary> Generate unique descriptions </summary>
        /// <param name="view">Accpac Business View</param>
        /// <param name="uniqueDescriptions">Dictionary of unique descriptions</param>
        private static void GenerateUniqueDescriptions(View view, Dictionary<string, bool> uniqueDescriptions)
        {
            uniqueDescriptions.Clear();

            // Iterate Accpac View
            for (var i = 0; i < view.Fields.Count; i++)
            {
                // Ignore those fields having description "RESERVED" OR "INTERNAL USE"
                if (view.Fields[i].Description.ToUpper().StartsWith("RESERVED") ||
                    view.Fields[i].Description.ToUpper().StartsWith("INTERNAL USE"))
                {
                    continue;
                }

                if (!uniqueDescriptions.ContainsKey(view.Fields[i].Description))
                {
                    uniqueDescriptions.Add(view.Fields[i].Description, true);
                }
                else
                {
                    uniqueDescriptions[view.Fields[i].Description] = false;
                }
            }
        }

        /// <summary> Get value from presentation list </summary>
        /// <param name="i">Outer Loop</param>
        /// <param name="j">Inner Loop</param>
        /// <param name="view">Accpac Business View</param>
        private static Object GetValue(int i, int j, View view)
        {
            if (Int32.TryParse(view.Fields[i].PresentationList.PredefinedValue(j).ToString(), out int intVal))
            {
                return intVal;
            }
            if (bool.TryParse(view.Fields[i].PresentationList.PredefinedValue(j).ToString(), out bool boolVal))
            {
                return boolVal ? 1 : 0;
            }

            return Convert.ToChar(view.Fields[i].PresentationList.PredefinedValue(j));
        }

        /// <summary>
        /// Get field's data type
        /// 
        /// This method will attempt to map Visual Basic data types (ViewFieldType)
        /// to C# data types (BusinessDataType)
        /// </summary>
        /// <param name="field">The field the data type is to be determined</param>
        /// <returns>Data type of the field</returns>
        private static BusinessDataType FieldType(ViewField field)
        {
            if (field.PresentationType == ViewFieldPresentationType.List)
            {
                return BusinessDataType.Enumeration;
            }

            // Need to use enum field.Type.HasFlag
            var hc = field.Type.GetHashCode();
            if (hc == ViewFieldType.Long.GetHashCode())    { return BusinessDataType.Integer; }
            if (hc == ViewFieldType.Char.GetHashCode())    { return BusinessDataType.String; }
            if (hc == ViewFieldType.Date.GetHashCode())    { return BusinessDataType.DateTime; }
            if (hc == ViewFieldType.Int.GetHashCode())     { return BusinessDataType.Short; }
            if (hc == ViewFieldType.Decimal.GetHashCode()) { return BusinessDataType.Decimal; }
            if (hc == ViewFieldType.Bool.GetHashCode())    { return BusinessDataType.Boolean; }
            if (hc == ViewFieldType.Time.GetHashCode())    { return BusinessDataType.TimeSpan; }
            if (hc == ViewFieldType.Byte.GetHashCode())    { return BusinessDataType.Byte; }

            return BusinessDataType.Double;
        }

        /// <summary> Get field's data type </summary>
        /// <param name="field">The field the data type is to be determined</param>
        /// <returns>Data type of the field</returns>
        private static BusinessDataType FieldTypeForWebApi(ViewField field)
        {
            //Need to use enum field.Type.HasFlag
            var viewfieldType = field.Type;
            var typeHash = viewfieldType.GetHashCode();

            if (typeHash == ViewFieldType.Bool.GetHashCode())
            {
                return BusinessDataType.Boolean;
            }
            if (field.PresentationType == ViewFieldPresentationType.List)
            {
                if (typeHash != ViewFieldType.Char.GetHashCode() || field.Size == 1)
                    return BusinessDataType.Enumeration;
            }
            if (typeHash == ViewFieldType.Long.GetHashCode())
            {
                return BusinessDataType.Long;
            }
            if (typeHash == ViewFieldType.LongLong.GetHashCode())
            {
                return BusinessDataType.Long;
            }
            if (typeHash == ViewFieldType.Char.GetHashCode())
            {
                return BusinessDataType.String;
            }
            if (typeHash == ViewFieldType.Date.GetHashCode())
            {
                return BusinessDataType.DateTime;
            }
            if (typeHash == ViewFieldType.Int.GetHashCode())
            {
                return BusinessDataType.Integer;
            }
            if (typeHash == ViewFieldType.Decimal.GetHashCode())
            {
                return BusinessDataType.Decimal;
            }
            if (typeHash == ViewFieldType.Time.GetHashCode())
            {
                return BusinessDataType.DateTime;
            }
            if (typeHash == ViewFieldType.Byte.GetHashCode())
            {
                return BusinessDataType.Byte;
            }

            return BusinessDataType.Double;
        }

        /// <summary> ValidateFields</summary>
        /// <param name="entityFields">the list of BusinessFields to iterate</param>
        /// <param name="uniqueDescriptions">Dictionary of unique descriptions</param>
        /// <param name="repositoryType">The repository type</param>
        /// <returns> String.Empty if valid otherwise message</returns>
        public static string ValidateFields(List<BusinessField> entityFields, Dictionary<string, bool> uniqueDescriptions, RepositoryType repositoryType)
        {
            var validFields = string.Empty;
            uniqueDescriptions.Clear();

            // Iterate fields
            for (var i = 0; i < entityFields.Count; i++)
            {
                // Locals
                var field = entityFields[i];

                // Check Name
                if (string.IsNullOrEmpty(field.Name))
                {
                    validFields = Resources.InvalidFieldBlank;
                    break;
                }

                // Ensure Name is properly formatted
                field.Name = BusinessViewHelper.Replace(field.Name);

                if (repositoryType.Equals(RepositoryType.Report))
                {
                    field.Description = field.Name;
                }

                // Ensure name is unique
                if (uniqueDescriptions.ContainsKey(field.Name))
                {
                    // Duplicate name entered
                    validFields = string.Format(Resources.InvalidFieldDuplicate, field.Name);
                    break;
                }

                // Add for next check
                uniqueDescriptions.Add(field.Name, false);
            }

            return validFields;
        }

        /// <summary> Generate enums </summary>
        /// <param name="businessView">Business View</param>
        /// <param name="view">Accpac Business View</param>
        /// <param name="uniqueDescriptions">Dictionary of unique descriptions</param>
        private static void GenerateFieldsAndEnums(BusinessView businessView, View view, Dictionary<string, bool> uniqueDescriptions, WizardType wizardType)
        {
            // Iterate Accpac View
            for (var i = 0; i < view.Fields.Count; i++)
            {
                // Ignore those fields having description "RESERVED" OR "INTERNAL USE"
                if (view.Fields[i].Description.ToUpper().StartsWith("RESERVED") ||
                    view.Fields[i].Description.ToUpper().StartsWith("INTERNAL USE"))
                {
                    continue;
                }

                var field = view.Fields[i];

                var type = (wizardType == WizardType.WEB)? FieldType(field): FieldTypeForWebApi(field);

                var isNumeric = type == BusinessDataType.Decimal ||
                                type == BusinessDataType.Double ||
                                type == BusinessDataType.Integer ||
                                type == BusinessDataType.Long ||
                                type == BusinessDataType.Short;

                var businessField = new BusinessField
                {
                    ServerFieldName = field.Name,
                    Name = FieldName(field, businessView.Properties[BusinessView.Constants.EntityName], uniqueDescriptions),
                    Description = field.Description,
                    Type = type,
                    Id = field.ID,
                    Size = field.Size,
                    IsReadOnly = !field.Attributes.HasFlag(ViewFieldAttributes.Editable),
                    IsCalculated = field.Attributes.HasFlag(ViewFieldAttributes.Calculated),
                    IsRequired = field.Attributes.HasFlag(ViewFieldAttributes.Required),
                    IsKey = field.Attributes.HasFlag(ViewFieldAttributes.Key),
                    IsUpperCase = field.PresentationMask != null && field.PresentationMask.Contains("C"),
                    IsAlphaNumeric = field.PresentationMask != null && field.PresentationMask.Contains("N"),
                    IsNumeric = isNumeric,
                    IsDynamicEnablement = field.Attributes.HasFlag(ViewFieldAttributes.CheckEditable),
#if ENABLE_TK_244885
                    IsCommon = false,
                    //AlternateName = string.Empty,
#endif
                    Precision = field.Precision,
                    MinValue = field.MinValue,
                    MaxValue = field.MaxValue,
                    EntityFieldType = Convert.ToInt32(field.Type),
                    Mask = field.PresentationMask,
                    ViewFieldType = field.Type
                };

                if (string.CompareOrdinal(businessField.Name, businessView.Properties[BusinessView.Constants.ModelName]) == 0 && businessField.IsKey)
                {
                    businessField.Name += "Key";
                }

                // No longer necessary (TK-253029)
                // Add to Keys if it is a key
                //if (businessField.IsKey)
                //{
                //    businessView.Keys.Add(businessField.Name);
                //}

                if (field.PresentationType == ViewFieldPresentationType.List)
                {
                    // Add previous name in case field name is changed in properties grid
                    businessField.PreviousName = businessField.Name;

                    var enumObject = new EnumHelper {
                        Name = businessField.Name,
#if ENABLE_TK_244885
                        IsCommon = businessField.IsCommon,
                        //AlternateName = businessField.AlternateName,
#endif                    
                    };

                    var builder = new StringBuilder();
                    for (var j = 0; j < field.PresentationList.Count; j++)
                    {
                        if (field.PresentationList.PredefinedString(j) != "N/A" || 
                            !string.IsNullOrEmpty(field.PresentationList.PredefinedString(j)))
                        {
                            // Will need to prefix key with value in case of dupes 
                            // (have only seen this with one view (IC0281)) and 
                            // thus will need to resolve in enum class when coding
                            var desc = field.PresentationList.PredefinedString(j);
                            var key = BusinessViewHelper.Replace(desc);
                            var value = GetValue(i, j, view);

                            // If the value coming from the presentation list is blank, assign it to None
                            if (string.IsNullOrEmpty(key))
                            {
                                key = "None";
                                desc = "None";
                            }

                            // Will strip out this prefix when building enum class
                            enumObject.Values.Add(value + ":" + key + ":" + desc, value);

                            builder.Append(field.PresentationList[j]);
                        }
                    }

                    businessView.Enums.Add(enumObject.Name, enumObject);
                }

                // Add to fields collection
                businessView.Fields.Add(businessField);
            }
        }

        /// <summary> Create a class using a T4 template </summary>
        /// <param name="view">Business View</param>
        /// <param name="settings">settings</param>
        /// <param name="templateClassName">template class name</param>
        /// <param name="element">XElement for UI Elements</param>
        /// <returns>string </returns>
        private static string TransformTemplateToText(BusinessView view, Settings settings, 
            string templateClassName, XElement element = null)
        {
            // instantiate a template class
            var type = Type.GetType("Sage.CA.SBS.ERP.Sage300.CodeGenerationWizard." + templateClassName);

            // Protection from incorrect class name
            if (type == null)
            {
                return string.Empty;
            }

            dynamic templateClassInstance = Activator.CreateInstance(type);

            templateClassInstance.Session = new Dictionary<string, object>();

            templateClassInstance.Session["view"] = view;
            templateClassInstance.Session["settings"] = settings;

            // UI Elements
            if (element != null)
            {
                templateClassInstance.Session["element"] = element;
            }

            templateClassInstance.Initialize();

            return templateClassInstance.TransformText();
        }

        /// <summary> Create a class for web api using a T4 template </summary>
        /// <param name="settings">settings</param>
        /// <param name="controllerSettings">Controller Settings</param>
        /// <param name="templateClassName">template class name</param>
        /// <returns>string </returns>
        private static string WebApiTransformTemplateToText(Settings settings, ControllerSettings controllerSettings, string templateClassName)
        {
            // instantiate a template class
            var type = Type.GetType("Sage.CA.SBS.ERP.Sage300.CodeGenerationWizard." + templateClassName);

            // Protection from incorrect class name
            if (type == null)
            {
                return string.Empty;
            }

            dynamic templateClassInstance = Activator.CreateInstance(type);

            templateClassInstance.Session = new Dictionary<string, object>();

            templateClassInstance.Session["settings"] = settings;
            templateClassInstance.Session["controllerSettings"] = controllerSettings;


            templateClassInstance.Initialize();

            return templateClassInstance.TransformText();
        }

        /// <summary> Iterate the view </summary>
        /// <param name="view">Business View</param>
        private void IterateView(BusinessView view)
        {
            // Set some flags to reduce code bulk a bit.
            var repoType = _settings.RepositoryType;
            var isRepoTypeHeaderDetail = repoType.Equals(RepositoryType.HeaderDetail);
            var isRepoTypeProcess = repoType.Equals(RepositoryType.Process);
            var isRepoTypeFlat = repoType.Equals(RepositoryType.Flat);
            var isRepoTypeReport = repoType.Equals(RepositoryType.Report);
            var generateFinder = view.Options[BusinessView.Constants.GenerateFinder];
            var generateClientFiles = view.Options[BusinessView.Constants.GenerateClientFiles];
            var entityName = view.Properties[BusinessView.Constants.EntityName];

            // Reduce code bulk a bit
            var specialFlag = (isRepoTypeHeaderDetail == false ||
                               isRepoTypeHeaderDetail == true && view.IsPartofHeaderDetailComposition);

            // TODO This needs to be investigated now that we are doing n entities (business views)

            BuildSubfolders(view);

            if (specialFlag)
            {
                // Create the Resx Files if the MenuResx file for that language exists
                CreateResxFilesByLanguage(_settings, view);

                // Create the Model class
                CreateClass(view,
                            entityName + ".cs",
                            TransformTemplateToText(view, _settings, "Templates.Common.Class.Model"),
                            Constants.ModelsKey, Constants.SubFolderModelKey);

                // Create the Model Mapper class 
                CreateClass(view,
                            entityName + "Mapper.cs",
                            TransformTemplateToText(view, _settings, "Templates.Common.Class.ModelMapper"),
                            Constants.BusinessRepositoryKey, Constants.SubFolderBusinessRepositoryMappersKey);
            }

            // Create the Model Fields class
            CreateClass(view,
                        entityName + "Fields.cs",
                        TransformTemplateToText(view, _settings, "Templates.Common.Class.ModelFields"),
                        Constants.ModelsKey, Constants.SubFolderModelFieldsKey);

            if (!isRepoTypeHeaderDetail)
            {
                if (generateClientFiles == true)
                {
                    // Create _Index.cshtml and _Localization.cshtml
                    var indexTemplate = "Templates.Common.View.Index";
                    var localizationTemplate = "Templates.Common.View.Localization";

                    if (isRepoTypeProcess)
                    {
                        indexTemplate = "Templates.Process.View.Index";
                        localizationTemplate = "Templates.Process.View.Localization";
                    }
                    else if (isRepoTypeReport)
                    {
                        indexTemplate = "Templates.Reports.View.Index";
                        localizationTemplate = "Templates.Reports.View.Localization";
                    }

                    CreateClass(view,
                                "Index.cshtml",
                                TransformTemplateToText(view, _settings, indexTemplate),
                                Constants.WebKey, Constants.SubFolderWebIndexKey);

                    CreateClass(view,
                                "_Localization.cshtml",
                                TransformTemplateToText(view, _settings, localizationTemplate),
                                Constants.WebKey, Constants.SubFolderWebLocalizationKey);

                    // Update the plugin menu details if not doing Payroll
                    if (_settings.ModuleId != "PR")
                    {
                        BusinessViewHelper.UpdateMenuDetails(view, _settings);
                    }

                }
            }

            if (specialFlag)
            {
                // A single Enumeration.cs file per directory
                var projectKey = Constants.ModelsKey;
                var subfolderKey = Constants.SubFolderModelEnumsKey;
                var projectInfo = _settings.Projects[projectKey][view.Properties[BusinessView.Constants.ModuleId]];
                var filePath = BusinessViewHelper.ConcatStrings(new[] { projectInfo.ProjectFolder, projectInfo.Subfolders[subfolderKey] });

                foreach (var value in view.Enums.Values)
                {
#if ENABLE_TK_244885
                    string content = string.Empty;
                    var enumName = value.Name;
                    _settings.EnumHelper = value;
                    var isCommon = value.IsCommon;
                    HandleEnumeration(isCommon, enumName, filePath, view);
#else
                    _settings.EnumHelper = value;

                    CreateClass(view,
                        value.Name + ".cs",
                        TransformTemplateToText(view, _settings, "Templates.Common.Class.ModelEnums"),
                        Constants.ModelsKey, Constants.SubFolderModelEnumsKey);
#endif
                }
            }

            CreateRepositoryClassesByRepositoryType(repoType, view);

            if (isRepoTypeHeaderDetail == false)
            {
                // Update security class
                BusinessViewHelper.UpdateSecurityClass(view, _settings);
            }
        }

        /// <summary> Process Enumerations marked as 'Common' </summary>
        /// <param name="enumName">The name of the enumeration</param>
        /// <param name="filename">The enumeration filename</param>
        /// <param name="filePath">The enumeration filepath</param>
        /// <param name="view">The BusinessView object reference</param>
        private void HandleCommonEnumeration(string enumName, 
                                             //string alternateName, 
                                             string filename, 
                                             string filePath, 
                                             BusinessView view)
        {
            #region Common Enumeration Example
            /* 
            /// <summary>
            /// Enum for Status
            /// </summary>
            public enum Status
            {
                /// <summary>
                /// Gets or sets Inactive
                /// </summary>
                [EnumValue("Inactive", typeof(CustomCommonResx))]
                Inactive = 0,
                /// <summary>
                /// Gets or sets Active
                /// </summary>
                [EnumValue("Active", typeof(CustomCommonResx))]
                Active = 1
            }
            */
            #endregion

            var content = string.Empty;
            var rootTemplateName = "Templates.Common.Class";
            var templateName = rootTemplateName + ".ModelCommonEnums";
            var templateNamePartial = rootTemplateName + ".ModelCommonEnumsPartial";
            var fileExists = File.Exists(filePath);

            if (fileExists == false)
            {
                // Generate content (complete file)
                content = TransformTemplateToText(view, _settings, templateName);

                // Create file with the content
                CreateClass(view, filename, content, Constants.ModelsKey, Constants.SubFolderModelEnumsKey);
            }
            else
            {
                // The file already exists so let's insert the new enumerations manually

                // Generate content (enum block only)
                content = TransformTemplateToText(view, _settings, templateNamePartial);

                InsertEnumBlockIfDoesNotExist(filePath, content);
            }
        }

        /// <summary> Process Enumerations not marked as 'Common' </summary>
        /// <param name="enumName">The name of the enumeration</param>
        /// <param name="filename">The enumeration filename</param>
        /// <param name="filePath">The enumeration filepath</param>
        /// <param name="view">The BusinessView object reference</param>
        private void HandleNormalEnumeration(string enumName, 
                                             //string alternateName, 
                                             string filename, 
                                             string filePath, 
                                             BusinessView view)
        {
            #region Normal Enumeration Example
            /* 
            /// <summary>
            /// Enum for PaymentType
            /// </summary>
            public enum PaymentType
            {
                /// <summary>
                /// Gets or sets Cash
                /// </summary>
                [EnumValue("Cash", typeof(PaymentCodeResx))]
                Cash = 1,
                /// <summary>
                /// Gets or sets Check
                /// </summary>
                [EnumValue("Check", typeof(PaymentCodeResx))]
                Check = 2,
                /// <summary>
                /// Gets or sets CreditCard
                /// </summary>
                [EnumValue("CreditCard", typeof(PaymentCodeResx))]
                CreditCard = 3,
                /// <summary>
                /// Gets or sets Other
                /// </summary>
                [EnumValue("Other", typeof(PaymentCodeResx))]
                Other = 4
            }
            */
            #endregion

            var content = string.Empty;
            var rootTemplateName = "Templates.Common.Class";
            var templateName = rootTemplateName + ".ModelEnums";
            var templateNamePartial = rootTemplateName + ".ModelEnumsPartial";

            var fileExists = File.Exists(filePath);

            if (fileExists == false)
            {
                // Generate content (complete file)
                content = TransformTemplateToText(view, _settings, templateName);

                // Create file with the content
                CreateClass(view, filename, content, Constants.ModelsKey, Constants.SubFolderModelEnumsKey);
            }
            else
            {
                // The file already exists so let's insert the new enumerations manually

                // Generate content (enum block only)
                content = TransformTemplateToText(view, _settings, templateNamePartial);

                InsertEnumBlockIfDoesNotExist(filePath, content);
            }
        }

        /// <summary> Insert an enumeration block into a file if it does not yet exist </summary>
        /// <param name="filePath">The file where the enumeration is located</param>
        /// <param name="content">The enumration block to insert</param>
        private void InsertEnumBlockIfDoesNotExist(string filePath, string content)
        {
            if (EnumBlockExistsInFile(filePath, content) == false)
            {
                InsertEnumBlock(filePath, content);
            }
        }

        /// <summary> A routine to handle enumerations </summary>
        /// <param name="isCommon">Is enumeration marked as common?</param>
        /// <param name="enumName">The name of the enumeration</param>
        /// <param name="filePath">The enumeration filepath</param>
        /// <param name="view">The BusinessView object reference</param>
        private void HandleEnumeration(bool isCommon, 
                                       //string alternateName, 
                                       string enumName, 
                                       string filePath,
                                       BusinessView view)
        {
            var filename = string.Empty;

            if (isCommon)
            {
                // Enumeration is marked as 'Common'
                filename = PrivateConstants.CommonEnumerationsFilename;
                filePath = BusinessViewHelper.ConcatStrings(new[] { filePath, filename });
                HandleCommonEnumeration(enumName, filename, filePath, view);
            }
            else
            {
                // Enumeration is NOT marked as 'Common'
                filename = PrivateConstants.RootEnumerationsFilename;
                filePath = BusinessViewHelper.ConcatStrings(new[] { filePath, filename });
                HandleNormalEnumeration(enumName, filename, filePath, view);
            }
        }

        /// <summary>
        /// This method will determine if an enumeration already exists in a file.
        /// It will check the name and all of the values.
        /// </summary>
        /// <param name="filepath">The file that will be processed</param>
        /// <param name="content">The content block containing an enumeration</param>
        /// <returns>true = Enumeration exists (name and values match) | false = Name and/or Value mismatch</returns>
        private bool EnumBlockExistsInFile(string filepath, string content)
        {
            if (File.Exists(filepath) == false) return false;

            // Build up the structure of the enumerations in the file
            var fileObject = ParseEnumerationsInFile(filepath);

            // Build up the structure of the enumeration in the content block
            var contentObject = ParseEnumerationsInBlock(content);

            return Utilities.Utilities.EnumExists(fileObject, contentObject);
        }

        /// <summary> Parse the enumerations located in a file </summary>
        /// <param name="filepath">The file to parse enumerations</param>
        /// <returns>The object containing a list of enumerations and the values associated with each</returns>
        private Dictionary<string, Dictionary<string, object>> ParseEnumerationsInFile(string filepath)
        {
            const string enumSignatureStub = @"public enum ";

            var container = new Dictionary<string, Dictionary<string, object>>();

            // Open the file and parse out the contents
            var allLines = File.ReadAllLines(filepath);
            var trimLines = allLines.Select(l => l.Trim()).ToList();

            var enumName = string.Empty;
            var processingEnum = false;

            var enumValueContainer = new Dictionary<string, object>();

            for (int x = 0; x < trimLines.Count; x++)
            {
                var currentLine = trimLines[x];
                if (currentLine.Contains(enumSignatureStub) && processingEnum == false)
                {
                    // Found the beginning of an enumeration

                    var sigArray = currentLine.Split(new string[] { " " }, StringSplitOptions.None);
                    enumName = sigArray[2];
                    processingEnum = true;
                }
                else if (processingEnum == true)
                {
                    // See if this line is actually one of the enumeration values
                    if (currentLine.Contains("="))
                    {
                        var valueLineArr = currentLine.Split('=');
                        var valueName = valueLineArr[0].Trim();
                        string value = valueLineArr[1].Trim();
                        value = value.RemoveLast(",");

                        // Add this value to the container
                        enumValueContainer.Add(valueName, value);
                    }
                    else if (currentLine.Contains("}"))
                    {
                        // We've reached the end of this enum block
                        // Add the enumeration to the container 
                        // and move on to the next one
                        container.Add(enumName, enumValueContainer);

                        // Empty the value bucket in preparation 
                        // for processing the next enumeration.
                        enumValueContainer.Clear();

                        // No longer processing an enumeration
                        processingEnum = false;
                    }
                }
            }

            return container;
        }

        /// <summary> Parse the enumerations located in a block of code </summary>
        /// <param name="content">The content block to parse for enumerations</param>
        /// <returns>The object containing a list of enumerations and the values associated with each</returns>
        private Dictionary<string, Dictionary<string, object>> ParseEnumerationsInBlock(string content)
        {
            const string enumSignatureStub = @"public enum ";

            var container = new Dictionary<string, Dictionary<string, object>>();

            // Open the file and parse out the contents
            var allLines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var trimLines = allLines.Select(l => l.Trim()).ToList();

            var enumName = string.Empty;
            var processingEnum = false;

            var enumValueContainer = new Dictionary<string, object>();

            for (int x = 0; x < trimLines.Count; x++)
            {
                var currentLine = trimLines[x];
                if (currentLine.Contains(enumSignatureStub) && processingEnum == false)
                {
                    // Found the beginning of an enumeration

                    var sigArray = currentLine.Split(new string[] { " " }, StringSplitOptions.None);
                    enumName = sigArray[2];
                    processingEnum = true;
                }
                else if (processingEnum == true)
                {
                    // See if this line is actually one of the enumeration values
                    if (currentLine.Contains("="))
                    {
                        var valueLineArr = currentLine.Split('=');
                        var valueName = valueLineArr[0].Trim();
                        string value = valueLineArr[1].Trim();
                        value = value.RemoveLast(",");

                        // Add this value to the container
                        enumValueContainer.Add(valueName, value);
                    }
                    else if (currentLine.Contains("}"))
                    {
                        // We've reached the end of this enum block
                        // Add the enumeration to the container 
                        // and move on to the next one
                        container.Add(enumName, enumValueContainer);
                        processingEnum = false;
                    }
                }
            }

            return container;
        }

        /// <summary> Insert an enumeration content block into a file. </summary>
        /// <param name="filePath">The file in which the enumeration will be inserted</param>
        /// <param name="content">The content block with the enumeration</param>
        private void InsertEnumBlock(string filepath, string content)
        {
            if (File.Exists(filepath) == true)
            {
                // Read the file into an array of strings
                var fileContents = File.ReadAllLines(filepath).ToList();

                // Get the last index in the file and insert the new content
                var insertionIndex = fileContents.Count - 1;
                fileContents.Insert(insertionIndex, content);

                // Save the file
                File.WriteAllLines(filepath, fileContents);
            }
        }

        /// <summary> Create Resx files based on the languages selected </summary>
        /// <param name="settings">The Settings object (for the language selections)</param>
        /// <param name="view">The BusinessView object</param>
        private void CreateResxFilesByLanguage(Settings settings, BusinessView view)
        {
#region Setup filenames
            var baseFilename = view.Properties[BusinessView.Constants.ResxName];

            // Common resources will live in only one location (Root of Resources project)
            var commonFilename = PrivateConstants.CustomCommonResx;
#endregion

#region Local Constants
            const int IndexBaseFile = 0;
            const int IndexCommonFile = 1;
#endregion

            // This is the list of resource files for each supported language 
            // (Normal resources and Common resources)
            var resourceFileNames = new string[][]
            {
                new string[] { baseFilename + GlobalConstants.ResourceFileExtension,
                               commonFilename + GlobalConstants.ResourceFileExtension },
                new string[] { baseFilename + "." + GlobalConstants.LanguageExtensions.Spanish + GlobalConstants.ResourceFileExtension,
                               commonFilename + "." + GlobalConstants.LanguageExtensions.Spanish + GlobalConstants.ResourceFileExtension },
                new string[] { baseFilename + "." + GlobalConstants.LanguageExtensions.French + GlobalConstants.ResourceFileExtension,
                               commonFilename + "." + GlobalConstants.LanguageExtensions.French + GlobalConstants.ResourceFileExtension },
                new string[] { baseFilename + "." + GlobalConstants.LanguageExtensions.ChineseSimplified + GlobalConstants.ResourceFileExtension,
                               commonFilename + "." + GlobalConstants.LanguageExtensions.ChineseSimplified + GlobalConstants.ResourceFileExtension },
                new string[] { baseFilename + "." + GlobalConstants.LanguageExtensions.ChineseTraditional + GlobalConstants.ResourceFileExtension,
                               commonFilename + "." + GlobalConstants.LanguageExtensions.ChineseTraditional + GlobalConstants.ResourceFileExtension },
            };

            // Create the resource files based on whether or not the language has been selected.
            if (settings.includeEnglish) { CreateResx(view, resourceFileNames[GlobalConstants.LanguageIndex.English][IndexBaseFile], 
                                                            resourceFileNames[GlobalConstants.LanguageIndex.English][IndexCommonFile], addDescription: true); }
            if (settings.includeSpanish) { CreateResx(view, resourceFileNames[GlobalConstants.LanguageIndex.Spanish][IndexBaseFile], 
                                                            resourceFileNames[GlobalConstants.LanguageIndex.Spanish][IndexCommonFile]); }
            if (settings.includeFrench) { CreateResx(view, resourceFileNames[GlobalConstants.LanguageIndex.French][IndexBaseFile], 
                                                           resourceFileNames[GlobalConstants.LanguageIndex.French][IndexCommonFile]); }
            if (settings.includeChineseSimplified) { CreateResx(view, resourceFileNames[GlobalConstants.LanguageIndex.ChineseSimplified][IndexBaseFile], 
                                                                      resourceFileNames[GlobalConstants.LanguageIndex.ChineseSimplified][IndexCommonFile]); }
            if (settings.includeChineseTraditional) { CreateResx(view, resourceFileNames[GlobalConstants.LanguageIndex.ChineseTraditional][IndexBaseFile], 
                                                                       resourceFileNames[GlobalConstants.LanguageIndex.ChineseTraditional][IndexCommonFile]); }
        }

        /// <summary> Create the repository classes by Repository Type </summary>
        /// <param name="type">The selected Repository Type enumeration value</param>
        /// <param name="view">The BusinessView</param>
        private void CreateRepositoryClassesByRepositoryType(RepositoryType type, BusinessView view)
        {
            //var isHeaderDetail = type.Equals(RepositoryType.HeaderDetail);
            var isProcess = type.Equals(RepositoryType.Process);
            var isFlat = type.Equals(RepositoryType.Flat);
            var isReport = type.Equals(RepositoryType.Report);

            if (isFlat) { CreateFlatRepositoryClasses(view); }
            if (isProcess) { CreateProcessRepositoryClasses(view); }
            if (isReport) { CreateReportRepositoryClasses(view); }
        }

        /// <summary> Create the class </summary>
        /// <param name="view">Business View</param>
        /// <param name="fileName"> Name of file to be created</param>
        /// <param name="content">File contents</param>
        /// <param name="projectKey">Project Key for Project Info</param>
        /// <param name="subfolderKey">Subfolder Key for Project Info</param>
        /// <param name="addToProject">True to add to project otherwise false</param>
        private void CreateClass(BusinessView view, string fileName, string content, string projectKey,
            string subfolderKey, bool addToProject = true, bool forGrid = false)
        {
            // Update display of file being processed
            ProcessingEvent?.Invoke(fileName);

            // Save the file
            var success = SaveFile(view, fileName, content, projectKey, subfolderKey, addToProject, forGrid);

            // Update status
            LaunchStatusEvent(success, fileName);
        }

        /// <summary> Update UI </summary>
        /// <param name="success">True/False based upon class generation</param>
        /// <param name="fileName">name of file to be created</param>
        private void LaunchStatusEvent(bool success, string fileName)
        {
            if (StatusEvent == null) { return; }
            Info.StatusTypeEnum statusType = success ? Info.StatusTypeEnum.Success : Info.StatusTypeEnum.Error;
            var message = success ? String.Empty : string.Format(Resources.ErrorCreatingFile, fileName);
            StatusEvent(fileName, statusType, message);
        }

        /// <summary> Create Flat Repository Classes </summary>
        /// <param name="view">Business View</param>
        private void CreateFlatRepositoryClasses(BusinessView view)
        {
            var generateClientFiles = view.Options[BusinessView.Constants.GenerateClientFiles];
            var entityName = view.Properties[BusinessView.Constants.EntityName];

            if (generateClientFiles)
            {

                if (view.Options[BusinessView.Constants.HasGrid])
                {
                    // Create the ViewModel class
                    CreateClass(view,
                                entityName + "ViewModel.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.Grid.ViewModel"),
                                Constants.WebKey, Constants.SubFolderWebViewModelKey);

                    // Create the Repository class
                    CreateClass(view,
                                entityName + "Repository.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.Grid.Repository"),
                                Constants.BusinessRepositoryKey, Constants.SubFolderBusinessRepositoryKey);

                    // Create the Business Repository Interface class
                    CreateClass(view,
                                "I" + entityName + "Repository.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.Grid.RepositoryInterface"),
                                Constants.InterfacesKey, Constants.SubFolderInterfacesBusinessRepositoryKey);


                    // Create the Internal Controller class
                    CreateClass(view,
                            entityName + "ControllerInternal.cs",
                            TransformTemplateToText(view, _settings, "Templates.Flat.Class.Grid.InternalController"),
                            Constants.WebKey, Constants.SubFolderWebControllersKey);

                    // Create the public Controller class
                    CreateClass(view,
                                entityName + "Controller.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.Grid.Controller"),
                                Constants.WebKey, Constants.SubFolderWebControllersKey);
                }
                else
                {
                    // Create the ViewModel class
                    CreateClass(view,
                                entityName + "ViewModel.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.ViewModel"),
                                Constants.WebKey, Constants.SubFolderWebViewModelKey);

                    // Create the Business Repository Interface class
                    CreateClass(view,
                                "I" + entityName + "Entity.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.RepositoryInterface"),
                                Constants.InterfacesKey, Constants.SubFolderInterfacesBusinessRepositoryKey);


                    // Create the Service Interface class
                    CreateClass(view,
                                "I" + entityName + "Service.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.ServiceInterface"),
                                Constants.InterfacesKey, Constants.SubFolderInterfacesServicesKey);

                    // Create the Service class
                    CreateClass(view,
                                entityName + "EntityService.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.Service"),
                                Constants.ServicesKey, Constants.SubFolderServicesKey);

                    // Create the Repository class
                    CreateClass(view,
                                entityName + "Repository.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.Repository"),
                                Constants.BusinessRepositoryKey, Constants.SubFolderBusinessRepositoryKey);

                    // Create the Internal Controller class
                    CreateClass(view,
                            entityName + "ControllerInternal.cs",
                            TransformTemplateToText(view, _settings, "Templates.Flat.Class.InternalController"),
                            Constants.WebKey, Constants.SubFolderWebControllersKey);

                    // Create the public Controller class
                    CreateClass(view,
                                entityName + "Controller.cs",
                                TransformTemplateToText(view, _settings, "Templates.Flat.Class.Controller"),
                                Constants.WebKey, Constants.SubFolderWebControllersKey);

                }
            }

            // Create partial view.cshtml
            if (generateClientFiles)
            {
                var fileName = "_" + entityName + ".cshtml";
                CreateClass(view,
                            fileName,
                            TransformTemplateToText(view, _settings, "Templates.Flat.View.Entity"),
                            Constants.WebKey, Constants.SubFolderWebLocalizationKey);

                // Create partial views for tabs, if any
                if (_settings.Widgets.ContainsKey("TabPage"))
                {
                    // Get tab pages
                    IEnumerable<XElement> tabPages =
                        from element in _settings.XmlLayout.Descendants("Control")
                        where (string)element.Attribute("widget") == "TabPage"
                        select element;

                    // Iterate tab pages to create partial views
                    foreach (XElement element in tabPages)
                    {
                        var pageId = element.Attribute("id").Value;
                        fileName = "_" + pageId + ".cshtml";
                        CreateClass(view,
                                    fileName,
                                    TransformTemplateToText(view, _settings, "Templates.Flat.View.PartialEntity", element),
                                    Constants.WebKey, Constants.SubFolderWebLocalizationKey);
                    }
                }

                // Create grid json files
                if (view.Options[BusinessView.Constants.HasGrid])
                {
                    fileName = view.Properties[BusinessView.Constants.EntityName] + "Grid.json";
                    CreateClass(view,
                                fileName,
                                TransformTemplateToText(view, _settings, "Templates.HeaderDetail.View.GridJson"),
                                Constants.WebKey, Constants.SubFolderWebLocalizationKey);
                }
            }

            // Register types
            BusinessViewHelper.UpdateFlatBootStrappers(view, _settings);
            if (generateClientFiles)
            {
                BusinessViewHelper.UpdateBundles(view, _settings);
            }

            // Add to constant.cs if partial views (Tab Control) 
            if (_settings.Widgets.ContainsKey("TabPage"))
            {
                BusinessViewHelper.UpdateConstants(view, _settings);
            }

            // set the start page
            if (generateClientFiles)
            {
                BusinessViewHelper.CreateViewPageUrl(view, _settings);
            }

            // Update the plugin menu details if not doing payroll
            if (_settings.ModuleId != "PR")
            {
                BusinessViewHelper.UpdateMenuDetails(view, _settings);
            }

            // For javascript files, the project name does not include the .Web segment
            if (generateClientFiles)
            {
                var projectName =
                _settings.Projects[Constants.WebKey][view.Properties[BusinessView.Constants.ModuleId]].ProjectName.Replace(".Web", string.Empty);

                if (view.Options[BusinessView.Constants.HasGrid])
                {
                    // Create the Behavior JavaScript file
                    CreateClass(view,
                            projectName + "." + entityName + "Behaviour.js",
                            TransformTemplateToText(view, _settings, "Templates.Flat.Script.Grid.Behaviour"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);

                }
                else
                {
                    // Create the Behavior JavaScript file
                    CreateClass(view,
                            projectName + "." + entityName + "Behaviour.js",
                            TransformTemplateToText(view, _settings, "Templates.Flat.Script.Behaviour"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);
                }

                // Create the Knockout Extension JavaScript file
                CreateClass(view,
                            projectName + "." + entityName + "KoExtn.js",
                            TransformTemplateToText(view, _settings, "Templates.Flat.Script.KoExtn"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);

                // Create the Repository JavaScript file
                CreateClass(view,
                            projectName + "." + entityName + "Repository.js",
                            TransformTemplateToText(view, _settings, "Templates.Flat.Script.Repository"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);
            }
        }

        /// <summary> Create Header-detail Repository Classes </summary>
        /// <param name="headerView">the header view</param>
        /// <param name="settings">settings</param>
        private void CreateHeaderDetailRepositoryClasses(BusinessView headerView, Settings settings)
        {
            var containerName = settings.EntitiesContainerName;

            // Create the Business Repository Interface class
            CreateClass(headerView, 
                "I" + containerName + "Repository.cs",
                TransformTemplateToText(headerView, settings, "Templates.HeaderDetail.Class.RepositoryInterface"),
                Constants.InterfacesKey, Constants.SubFolderInterfacesBusinessRepositoryKey);

            // Create the Repository class
            CreateClass(headerView,
                        containerName + "Repository.cs",
                        TransformTemplateToText(headerView, settings, "Templates.HeaderDetail.Class.Repository"),
                        Constants.BusinessRepositoryKey, Constants.SubFolderBusinessRepositoryKey);

            // Create the ViewModel class
            CreateClass(headerView,
                        containerName + "ViewModel.cs",
                        TransformTemplateToText(headerView, settings, "Templates.HeaderDetail.Class.ViewModel"),
                        Constants.WebKey, Constants.SubFolderWebViewModelKey);

            // Create the public Controller class
            CreateClass(headerView,
                        containerName + "Controller.cs",
                        TransformTemplateToText(headerView, settings, "Templates.HeaderDetail.Class.Controller"),
                        Constants.WebKey, Constants.SubFolderWebControllersKey);


            // Create the Internal Controller class
            CreateClass(headerView,
                        containerName + "ControllerInternal.cs",
                        TransformTemplateToText(headerView, settings, "Templates.HeaderDetail.Class.InternalController"),
                        Constants.WebKey, Constants.SubFolderWebControllersKey);

            // Create partial view.cshtml
            var fileName = "_" + containerName + ".cshtml";
            CreateClass(headerView,
                        fileName,
                        TransformTemplateToText(headerView, settings, "Templates.HeaderDetail.View.Entity"),
                        Constants.WebKey, Constants.SubFolderWebLocalizationKey);

            // Create partial views for tabs, if any
            if (_settings.Widgets.ContainsKey("TabPage"))
            {
                // Get tab pages
                IEnumerable<XElement> tabPages =
                    from element in _settings.XmlLayout.Descendants("Control")
                    where (string)element.Attribute("widget") == "TabPage"
                    select element;

                // Iterate tab pages to create partial views
                foreach (XElement element in tabPages)
                {
                    // Create tab page if specified regardless if any controls have been added
                    //if (element.HasElements)
                    //{
                        var pageId = element.Attribute("id").Value;
                        fileName = "_" + pageId + ".cshtml";
                        CreateClass(headerView,
                                    fileName,
                                    TransformTemplateToText(headerView, _settings, "Templates.HeaderDetail.View.PartialEntity", element),
                                    Constants.WebKey, Constants.SubFolderWebLocalizationKey);
                    //}
                }
            }
            // Create grid json files
            foreach (var view in settings.Entities)
            {
                if (view.Options[BusinessView.Constants.HasGrid])
                {
                    fileName = view.Properties[BusinessView.Constants.EntityName] + "Grid.json";
                    CreateClass(view,
                                fileName,
                                TransformTemplateToText(view, settings, "Templates.HeaderDetail.View.GridJson"),
                                Constants.WebKey, Constants.SubFolderWebLocalizationKey);
                }
            }

            // Add to constant.cs if partial views (Tab Control) 
            if (_settings.Widgets.ContainsKey("TabPage"))
            {
                BusinessViewHelper.UpdateConstants(headerView, _settings, true);
            }

            // Register types
            BusinessViewHelper.UpdateHeaderDetailBootStrappers(headerView, settings);

            BusinessViewHelper.UpdateBundles(headerView, settings);

            // Update security class
            BusinessViewHelper.UpdateSecurityClass(headerView, settings);

            // set the start page
            BusinessViewHelper.CreateViewPageUrl(headerView, settings);

            // Update the plugin menu details if not doing payroll
            if (_settings.ModuleId != "PR")
            {
                BusinessViewHelper.UpdateMenuDetails(headerView, settings);
            }

            // For javascript files, the project name does not include the .Web segment
            var projectName =
            settings.Projects[Constants.WebKey][headerView.Properties[BusinessView.Constants.ModuleId]].ProjectName.Replace(".Web", string.Empty);

            // Create the Behavior JavaScript file
            CreateClass(headerView,
                        projectName + "." + containerName + "Behaviour.js",
                        TransformTemplateToText(headerView, settings, "Templates.HeaderDetail.Script.Behaviour"),
                        Constants.WebKey, Constants.SubFolderWebScriptsKey);

            // Create the Knockout Extension JavaScript file
            CreateClass(headerView,
                        projectName + "." + containerName + "KoExtn.js",
                        TransformTemplateToText(headerView, settings, "Templates.Flat.Script.KoExtn"),
                        Constants.WebKey, Constants.SubFolderWebScriptsKey);

            // Create the Repository JavaScript file
            CreateClass(headerView,
                        projectName + "." + containerName + "Repository.js",
                        TransformTemplateToText(headerView, settings, "Templates.Flat.Script.Repository"),
                        Constants.WebKey, Constants.SubFolderWebScriptsKey);

            // Create _Index.cshtml
            CreateClass(headerView,
                        "Index.cshtml",
                        TransformTemplateToText(headerView, settings, "Templates.Common.View.Index"),
                        Constants.WebKey, Constants.SubFolderWebIndexKey);

            // Create _Localization.cshtml
            var localizationTemplate = "Templates.Common.View.Localization";
            CreateClass(headerView,
                        "_Localization.cshtml",
                        TransformTemplateToText(headerView, settings, localizationTemplate),
                        Constants.WebKey, Constants.SubFolderWebLocalizationKey);

            if (settings.includeEnglish)
            {
                CreateHeaderDetailResx(headerView, containerName + "Resx.resx", true);
            }
            if (settings.includeSpanish)
            {
                CreateHeaderDetailResx(headerView, containerName + "Resx.es.resx", false);
            }
            if (settings.includeFrench)
            {
                CreateHeaderDetailResx(headerView, containerName + "Resx.fr.resx", false);
            }
            if (settings.includeChineseSimplified)
            {
                CreateHeaderDetailResx(headerView, containerName + "Resx.zh-Hans.resx", false);
            }
            if (settings.includeChineseTraditional)
            {
                CreateHeaderDetailResx(headerView, containerName + "Resx.zh-Hant.resx", false);
            }
        }

        /// <summary> Create Process Repository Classes </summary>
        /// <param name="view">Business View</param>
        private void CreateProcessRepositoryClasses(BusinessView view)
        {
            var generateClientFiles = view.Options[BusinessView.Constants.GenerateClientFiles];
            var entityName = view.Properties[BusinessView.Constants.EntityName];

            // Create the Business Repository Interface class
            CreateClass(view,
                        "I" + entityName + "Entity.cs",
                        TransformTemplateToText(view, _settings, "Templates.Process.Class.RepositoryInterface"),
                        Constants.InterfacesKey, Constants.SubFolderInterfacesBusinessRepositoryKey);

            // Create the Service Interface class
            CreateClass(view,
                        "I" + entityName + "Service.cs",
                        TransformTemplateToText(view, _settings, "Templates.Process.Class.ServiceInterface"),
                        Constants.InterfacesKey, Constants.SubFolderInterfacesServicesKey);

            // Create the Service class
            CreateClass(view,
                        entityName + "Service.cs",
                        TransformTemplateToText(view, _settings, "Templates.Process.Class.Service"),
                        Constants.ServicesKey, Constants.SubFolderServicesKey);

            // Create the Unit of Work Service Class
            CreateClass(view,
                        entityName + "Uow.cs",
                        TransformTemplateToText(view, _settings, "Templates.Process.Class.Uow"),
                        Constants.ServicesKey, Constants.SubFolderUnitOfWorkKey);

            if (generateClientFiles)
            {
                // Create the ViewModel class
                CreateClass(view,
                            entityName + "ViewModel.cs",
                            TransformTemplateToText(view, _settings, "Templates.Process.Class.ViewModel"),
                            Constants.WebKey, Constants.SubFolderWebViewModelKey);

                // Create the Internal Controller class
                CreateClass(view,
                            entityName + "ControllerInternal.cs",
                            TransformTemplateToText(view, _settings, "Templates.Process.Class.InternalController"),
                            Constants.WebKey, Constants.SubFolderWebControllersKey);

                // Create the public Controller class
                CreateClass(view,
                            entityName + "Controller.cs",
                            TransformTemplateToText(view, _settings, "Templates.Process.Class.Controller"),
                            Constants.WebKey, Constants.SubFolderWebControllersKey);
            }

            // Create the Repository class
            CreateClass(view,
                        entityName + "Repository.cs",
                        TransformTemplateToText(view, _settings, "Templates.Process.Class.Repository"),
                        Constants.BusinessRepositoryKey, Constants.SubFolderBusinessRepositoryKey);

            // Create partial view.cshtml
            if (generateClientFiles)
            {
                var fileName = "_" + entityName + ".cshtml";
                CreateClass(view,
                            fileName,
                            TransformTemplateToText(view, _settings, "Templates.Process.View.Entity"),
                            Constants.WebKey, Constants.SubFolderWebLocalizationKey);

                // Create partial views for tabs, if any
                if (_settings.Widgets.ContainsKey("TabPage"))
                {
                    // Get tab pages
                    IEnumerable<XElement> tabPages =
                        from element in _settings.XmlLayout.Descendants("Control")
                        where (string)element.Attribute("widget") == "TabPage"
                        select element;

                    // Iterate tab pages to create partial views
                    foreach (XElement element in tabPages)
                    {
                        var pageId = element.Attribute("id").Value;
                        fileName = "_" + pageId + ".cshtml";
                        CreateClass(view,
                                    fileName,
                                    TransformTemplateToText(view, _settings, "Templates.Process.View.PartialEntity", element),
                                    Constants.WebKey, Constants.SubFolderWebLocalizationKey);
                    }
                }
            }

            // Register types
            BusinessViewHelper.UpdateProcessBootStrappers(view, _settings);
            if (generateClientFiles)
            {
                BusinessViewHelper.UpdateBundles(view, _settings);
            }

            // Add to constant.cs if partial views (Tab Control) 
            if (_settings.Widgets.ContainsKey("TabPage"))
            {
                BusinessViewHelper.UpdateConstants(view, _settings);
            }

            // set the start page
            if (generateClientFiles)
            {
                BusinessViewHelper.CreateViewPageUrl(view, _settings);
            }

            // Update the plugin menu details if not doing payroll
            if (_settings.ModuleId != "PR")
            {
                BusinessViewHelper.UpdateMenuDetails(view, _settings);
            }

            // For javascript files, the project name does not include the .Web segment
            if (generateClientFiles)
            {
                var projectName =
                _settings.Projects[Constants.WebKey][view.Properties[BusinessView.Constants.ModuleId]].ProjectName.Replace(".Web", string.Empty);

                // Create the Behavior JavaScript file
                CreateClass(view,
                            projectName + "." + entityName + "Behaviour.js",
                            TransformTemplateToText(view, _settings, "Templates.Process.Script.Behaviour"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);

                // Create the Knockout Extension JavaScript file
                CreateClass(view,
                            projectName + "." + entityName + "KoExtn.js",
                            TransformTemplateToText(view, _settings, "Templates.Process.Script.KoExtn"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);

                // Create the Repository JavaScript file
                CreateClass(view,
                            projectName + "." + entityName + "Repository.js",
                            TransformTemplateToText(view, _settings, "Templates.Process.Script.Repository"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);
            }

            // Create the SQL script
            CreateClass(view,
                        entityName + "_WorkerRole_Data.sql",
                        TransformTemplateToText(view, _settings, "Templates.Process.Script.Sql"),
                        Constants.WebKey, Constants.SubFolderWebSqlKey,
                        false);
        }

        /// <summary> Create Report Repository Classes </summary>
        /// <param name="view">Business View</param>
        private void CreateReportRepositoryClasses(BusinessView view)
        {
            var generateClientFiles = view.Options[BusinessView.Constants.GenerateClientFiles];
            var entityName = view.Properties[BusinessView.Constants.EntityName];

            // Create the Business Repository Interface class
            CreateClass(view,
                        "I" + entityName + "Entity.cs",
                        TransformTemplateToText(view, _settings, "Templates.Reports.Class.RepositoryInterface"),
                        Constants.InterfacesKey, Constants.SubFolderInterfacesBusinessRepositoryKey);

            // Create the Service Interface class
            CreateClass(view,
                        "I" + entityName + "Service.cs",
                        TransformTemplateToText(view, _settings, "Templates.Reports.Class.ServiceInterface"),
                        Constants.InterfacesKey, Constants.SubFolderInterfacesServicesKey);

            // Create the Service class
            CreateClass(view,
                        entityName + "EntityService.cs",
                        TransformTemplateToText(view, _settings, "Templates.Reports.Class.Service"),
                        Constants.ServicesKey, Constants.SubFolderServicesKey);

            if (generateClientFiles)
            {
                // Create the ViewModel class
                CreateClass(view,
                            entityName + "ViewModel.cs",
                            TransformTemplateToText(view, _settings, "Templates.Reports.Class.ViewModel"),
                            Constants.WebKey, Constants.SubFolderWebViewModelKey);

                // Create the public Controller class
                CreateClass(view,
                            entityName + "Controller.cs",
                            TransformTemplateToText(view, _settings, "Templates.Reports.Class.Controller"),
                            Constants.WebKey, Constants.SubFolderWebControllersKey);

                // Create the internal Controller class
                CreateClass(view,
                            entityName + "ControllerInternal.cs",
                            TransformTemplateToText(view, _settings, "Templates.Reports.Class.InternalController"),
                            Constants.WebKey, Constants.SubFolderWebControllersKey);
            }

            // Create the Repository class
            CreateClass(view,
                        entityName + "Repository.cs",
                        TransformTemplateToText(view, _settings, "Templates.Reports.Class.Repository"),
                        Constants.BusinessRepositoryKey, Constants.SubFolderBusinessRepositoryKey);

            // Create partial view.cshtml
            if (generateClientFiles)
            {
                var fileName = "_" + entityName + ".cshtml";
                CreateClass(view,
                            fileName,
                            TransformTemplateToText(view, _settings, "Templates.Reports.View.Entity"),
                            Constants.WebKey, Constants.SubFolderWebLocalizationKey);

                // Create partial views for tabs, if any
                if (_settings.Widgets.ContainsKey("TabPage"))
                {
                    // Get tab pages
                    IEnumerable<XElement> tabPages =
                        from element in _settings.XmlLayout.Descendants("Control")
                        where (string)element.Attribute("widget") == "TabPage"
                        select element;

                    // Iterate tab pages to create partial views
                    foreach (XElement element in tabPages)
                    {
                        var pageId = element.Attribute("id").Value;
                        fileName = "_" + pageId + ".cshtml";
                        CreateClass(view,
                                    fileName,
                                    TransformTemplateToText(view, _settings, "Templates.Reports.View.PartialEntity", element),
                                    Constants.WebKey, Constants.SubFolderWebLocalizationKey);
                    }
                }
            }

            // For javascript files, the project name does not include the .Web segment
            if (generateClientFiles)
            {
                var projectName =
                _settings.Projects[Constants.WebKey][view.Properties[BusinessView.Constants.ModuleId]].ProjectName.Replace(".Web", string.Empty);

                // Create the Behavior JavaScript file
                CreateClass(view,
                            projectName + "." + entityName + "Behaviour.js",
                            TransformTemplateToText(view, _settings, "Templates.Reports.Script.Behaviour"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);

                // Create the Knockout Extension JavaScript file
                CreateClass(view,
                            projectName + "." + entityName + "KoExtn.js",
                            TransformTemplateToText(view, _settings, "Templates.Reports.Script.KoExtn"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);

                // Create the Repository JavaScript file
                CreateClass(view,
                            projectName + "." + entityName + "Repository.js",
                            TransformTemplateToText(view, _settings, "Templates.Reports.Script.Repository"),
                            Constants.WebKey, Constants.SubFolderWebScriptsKey);
            }

            // Register types
            BusinessViewHelper.UpdateReportBootStrappers(view, _settings);
            if (generateClientFiles)
            {
                BusinessViewHelper.UpdateBundles(view, _settings);
            }

            // Add to constant.cs if partial views (Tab Control) 
            if (_settings.Widgets.ContainsKey("TabPage"))
            {
                BusinessViewHelper.UpdateConstants(view, _settings);
            }

            // set the start page
            if (generateClientFiles)
            {
                BusinessViewHelper.CreateViewPageUrl(view, _settings);
            }
        }

        /// <summary> Create the HeaderDetail Resx content </summary>
        /// <param name="view">Business View</param>
        /// <param name="fileName">Resx File Name</param>
        /// <param name="addDescription">True to add descriptions otherwise false</param>
        private void CreateHeaderDetailResx(BusinessView view, string fileName, bool addDescription)
        {
            // Update display of file being processed
            ProcessingEvent?.Invoke(fileName);

            // Vars
            var retVal = true;
            var projectInfo = _settings.Projects[Constants.ResourcesKey][view.Properties[BusinessView.Constants.ModuleId]];
            var filePath = BusinessViewHelper.ConcatStrings(new[] { projectInfo.ProjectFolder, projectInfo.Subfolders[Constants.SubFolderResourcesKey] });
            var fullFileName = BusinessViewHelper.ConcatStrings(new[] { filePath, fileName });

            // Determine if exists
            var exists = File.Exists(fullFileName);
            var writeFile = true;

            // Prompt if file exists?
            if (exists && _settings.PromptIfExists)
            {
                var result = MessageBox.Show(string.Format(Resources.FileExists, fullFileName),
                    Resources.Confirmation, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                // Evaluate
                switch (result)
                {
                    case DialogResult.Yes:
                        // Overwrite the file
                        break;
                    case DialogResult.No:
                        // Skip the file
                        writeFile = false;
                        break;
                    case DialogResult.Cancel:
                        // Abort the wizard
                        throw new Exception();
                }
            }

            // Write the file?
            if (writeFile)
            {
                try
                {
                    // Ensure the filepath exists
                    if (!exists)
                    {
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                    }

                    // Create resx file
                    var resx = new ResXResourceWriter(fullFileName);
                    var uniqueList = new List<string>();

                    // Add Enity key
                    resx.AddResource(new ResXDataNode("Entity", addDescription ? view.Properties[BusinessView.Constants.ModuleId] +
                        " " + _settings.EntitiesContainerName : string.Empty));

                    resx.Close();
                }
                catch
                {
                    retVal = false;
                }

                // Update project if write was successful
                if (retVal)
                {
                    // Add to project
                    try
                    {
                        var createdItem = projectInfo.Project.ProjectItems.AddFromFile(fullFileName);

                        // Only add code behind file for english resx file
                        if (addDescription)
                        {
                            createdItem.Properties.Item("CustomTool").Value = "PublicResXFileCodeGenerator";
                        }
                    }
                    catch
                    {
                        retVal = false;
                    }
                }

            }
            else
            {
                retVal = false;
            }


            // Update status
            LaunchStatusEvent(retVal, fileName);
        }


        /// <summary> Create the Resx content Both discrete and common </summary>
        /// <param name="view">Business View</param>
        /// <param name="filename">The resource file name</param>
        /// <param name="commonFilename">The common resource file name</param>
        /// <param name="addDescription">True to add descriptions otherwise false (default is false)</param>
        private void CreateResx(BusinessView view, string filename, string commonFilename, bool addDescription = false)
        {
            // Process the regular Resx file (skip entities not marked as 'IsCommon')
            var fileToProcess = filename;
            ProcessingEvent?.Invoke(fileToProcess);
            var success = SaveResxFile(isStandardResxFile: true, 
                                       view: view, 
                                       fileName: fileToProcess, 
                                       addDescription: addDescription);
            LaunchStatusEvent(success, fileToProcess);

#if ENABLE_TK_244885
            if (success)
            {
                // Process the Common Resx file (include only entities marked as 'IsCommon')
                fileToProcess = commonFilename;
                ProcessingEvent?.Invoke(fileToProcess);
                success = SaveResxFile(isStandardResxFile: false,
                                       view: view,
                                       fileName: fileToProcess,
                                       addDescription: addDescription);
                LaunchStatusEvent(success, fileToProcess);
            }
#endif
        }

        /// <summary> Save Resx File </summary>
        /// <param name="isStandardResxFile"></param>
        /// <param name="view"></param>
        /// <param name="fileName"></param>
        /// <param name="addDescription"></param>
        /// <returns></returns>
        private bool SaveResxFile(bool isStandardResxFile, BusinessView view, string fileName, bool addDescription)
        {
            // Variables
#if ENABLE_TK_244885
            var isCommon = !isStandardResxFile;
#else
            var isCommon = false;
#endif
            var fileWriteSuccessful = true;
            var projectInfo = GetProjectInfo(view);
#if ENABLE_TK_244885
            var filePath = isCommon ? projectInfo.ProjectFolder
                                    : BusinessViewHelper.ConcatStrings(new[] { projectInfo.ProjectFolder, projectInfo.Subfolders[Constants.SubFolderResourcesKey] });
#else
            var filePath = BusinessViewHelper.ConcatStrings(new[] { projectInfo.ProjectFolder, projectInfo.Subfolders[Constants.SubFolderResourcesKey] });
#endif
            var resourceFileName = BusinessViewHelper.ConcatStrings(new[] { filePath, fileName });
            var writeFile = true;

            // Determine if the resource file exists or not
            var fileExists = File.Exists(resourceFileName);

            var resourceManager = new Utilities.ResXManager(resourceFileName);

            // Prompt if file exists?
            if (writeFile = PromptIfFileExists(fileExists, _settings.PromptIfExists, resourceFileName) == false)
            {
                return false;
            }

            try
            {
                var uniqueList = new List<string>();

                // If file doesn't exist, 
                // ensure that the directory exists
                if (!fileExists)
                {
                    CreateDirectoryIfNotYetExists(filePath);
                }

                // Only necessary if processing a normal resource file
                if (isCommon == false)
                {
                    // Add Entity key
                    var key = "Entity";
                    var value = addDescription ? view.Properties[BusinessView.Constants.ModuleId] +
                                                 " " + view.Properties[BusinessView.Constants.EntityName] : string.Empty;
                    resourceManager.InsertIfNotExist(key, value);
                }


#if ENABLE_TK_244885
                // Iterate fields collection (filter by Enumerations marked as 'IsCommon' (or not))
                var results = view.Fields.Where(field => field.IsCommon == isCommon);
                var nodes = from field in results
                            where !uniqueList.Contains(field.Name, StringComparer.CurrentCultureIgnoreCase)
                            select new { field.Name, field.Description };
#else
                var nodes = from field in view.Fields
                            where !uniqueList.Contains(field.Name, StringComparer.CurrentCultureIgnoreCase)
                            select new { field.Name, field.Description };
#endif

                foreach (var node in nodes)
                {
                    var name = node.Name;
                    var value = addDescription ? node.Description : string.Empty;
                    resourceManager.InsertIfNotExist(name, value);

                    // Add the enum name to the list of unique strings
                    uniqueList.Add(name);
                }

                // Iterate tab pages, if any
                if (_settings.Widgets.ContainsKey("TabPage"))
                {
                    // Get tab pages
                    IEnumerable<XElement> tabPages =
                        from element in _settings.XmlLayout.Descendants("Control")
                        where (string)element.Attribute("widget") == "TabPage"
                        select element;

                    // Iterate tab pages to create partial views
                    foreach (XElement element in tabPages)
                    {
                        var name = element.Attribute("id").Value;
                        var value = addDescription ? element.Attribute("text").Value : string.Empty;
                        resourceManager.InsertIfNotExist(name, value);

                        // Add to the list of unique strings
                        uniqueList.Add(name);
                    }
                }

                // Iterate the actual enumeration values
                // We dealt with the enumeration names in the previous code block.
                foreach (var enumHelper in view.Enums.Values)
                {
                    foreach (var value in enumHelper.Values)
                    {
                        // Locals - Used to split out prefix and replace invalid characters
                        GetEnumKeyAndDescription(value, out string key, out string description);

                        // Add only if not yet present and not blank
                        if (KeyIsPresentOrBlank(key, uniqueList) == false)
                        {
#if ENABLE_TK_244885
                            if (enumHelper.IsCommon == isCommon)
                            {
                                resourceManager.InsertIfNotExist(key, addDescription ? description : string.Empty);
                            }
#else
                            resourceManager.InsertIfNotExist(key, addDescription ? description : string.Empty);
#endif
                            uniqueList.Add(key);
                        }
                    }
                }

                _settings.ResourceKeys = uniqueList;
                resourceManager.Write();
            }
            catch
            {
                fileWriteSuccessful = false;
            }

            if (fileWriteSuccessful)
            {
                try
                {
                    AddResourceFileToProject(projectInfo, resourceFileName, addDescription);
                }
                catch
                {
                    fileWriteSuccessful = false;
                }
            }

            return fileWriteSuccessful;
        }
		
        /// <summary> Get the ProjectInfo object </summary>
        /// <returns>Returns the ProjectInfo object.</returns>
        private ProjectInfo GetProjectInfo(BusinessView v) => 
            _settings.Projects[Constants.ResourcesKey][v.Properties[BusinessView.Constants.ModuleId]];

        /// <summary> Create a directory if it doesn't yet exist </summary>
        /// <param name="filePath">The path to the folder to create</param>
        private void CreateDirectoryIfNotYetExists(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }
		
        /// <summary> Add a resource file to a Visual Studio project </summary>
        /// <param name="pi">The ProjectInfo object</param>
        /// <param name="file">The path to the resource file</param>
        /// <param name="addDescription">The flag</param>
        private void AddResourceFileToProject(ProjectInfo pi, string file, bool addDescription)
        {
            var createdItem = pi.Project.ProjectItems.AddFromFile(file);

            // Only add code behind file for english resx file
            if (addDescription)
            {
                createdItem.Properties.Item("CustomTool").Value = "PublicResXFileCodeGenerator";
            }

            pi.Project.Save(pi.Project.FullName);
        }

        /// <summary> Display a prompt if a file already exists </summary>
        /// <param name="fileExists">The flag denoting whether or not a file already exists.</param>
        /// <param name="promptIfExists">The flag denoting whether or not to display a prompt.</param>
        /// <param name="filename">The file whose existence is checked</param>
        /// <returns></returns>
        private bool PromptIfFileExists(bool fileExists, bool promptIfExists, string filename)
        {
            var writeFile = true;

            if (fileExists && promptIfExists)
            {
                var result = MessageBox.Show(string.Format(Resources.FileExists, filename),
                                             Resources.Confirmation,
                                             MessageBoxButtons.YesNoCancel,
                                             MessageBoxIcon.Question);

                // Evaluate
                switch (result)
                {
                    case DialogResult.Yes:
                        // Overwrite the file
                        break;
                    case DialogResult.No:
                        // Skip the file
                        writeFile = false;
                        break;
                    case DialogResult.Cancel:
                        // Abort the wizard
                        throw new Exception();
                }
            }

            return writeFile;
        }

        /// <summary> A routine to parse a KeyValuePair </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="key">Output parameter key</param>
        /// <param name="description">Output parameter description</param>
        private void GetEnumKeyAndDescription(KeyValuePair<string, object> value, out string key, out string description)
        {
            var arr = value.Key.Split(':');
            key = arr[1]; // Replace function already performed
            description = arr[2]; // Raw from presentation list
        }

        /// <summary>
        /// A routine to determine if a string key is null (or empty)
        /// OR if a list contains an entry designated by a key.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <param name="list">The list to check the key against</param>
        /// <returns>
        /// true : key is blank OR list contains an entry for key 
        /// false : key is not blank OR list does not contain entry for key.
        /// </returns>
        private bool KeyIsPresentOrBlank(string key, List<string> list) => 
            string.IsNullOrEmpty(key) || list.Contains(key, StringComparer.CurrentCultureIgnoreCase);

#endregion

    }
}
