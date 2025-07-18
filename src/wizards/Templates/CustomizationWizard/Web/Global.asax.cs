﻿/* Copyright (c) 1994-2025 Sage Software, Inc.  All rights reserved. */

#region

using Sage.CA.SBS.ERP.Sage300.Common.Interfaces.Bootstrap;
using Sage.CA.SBS.ERP.Sage300.Common.Models;
using Sage.CA.SBS.ERP.Sage300.Common.Services;
using Sage.CA.SBS.ERP.Sage300.Common.Utilities;
using Sage.CA.SBS.ERP.Sage300.Common.Web.Security;
using Sage.CA.SBS.ERP.Sage300.Core.Logging;
using Sage.CA.SBS.ERP.Sage300.Web;
using Sage.CA.SBS.ERP.Sage300.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

#endregion

namespace $safeprojectname$
{
    /// <summary>
    /// MVC application class that provides start and end functionality for application and user sessions
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        private bool _isAuthenticated = false;

        private void Session_Start(object sender, EventArgs e)
        {
            if (!_isAuthenticated)
            {
                var authenticationManager = new AuthenticationManagerOnPremise();
                authenticationManager.Login();
                var recordId = Guid.NewGuid();
                var context = new Context
                {
                    AspNetSessionId = HttpContext.Current.Session.SessionID,
                    ApplicationUserId = "ADMIN",
                    Company = "SAMLTD",
                    ProductUserId = recordId,
                    TenantId = recordId,
                    TenantAlias = Sage.CA.SBS.ERP.Sage300.Common.Web.AreaConstants.Core.OnPremiseTenantAlias,
                    ApplicationType = ApplicationType.WebApplication,
                    Language = "en",
                    ScreenName = "None",
                    Container = BootstrapTaskManager.Container,
					ScreenContext = new ScreenContext(),
                };

                var sessionId = $"{context.ApplicationUserId.Trim()}-{context.Company.Trim()}";
                context.ScreenContext.ScreenName = "None";
                context.SessionId = Encoding.UTF8.Base64Encode(sessionId);
				
                //Set default company information
                var companies = new List<Organization>
                {
                    new Organization() { Id ="SAMLTD", Name = "SAMLTD", SystemId = "SAMSYS", System = "SAMSYS", IsSecurityEnabled = false }
                };

                authenticationManager.LoginResult("SAMLTD", "ADMIN", "ADMIN", BootstrapTaskManager.Container, context, companies);
                _isAuthenticated = true;

                //Redirect to the last generated page
                var fileUrlPath = Path.Combine(Server.MapPath("~"), "PageUrl.txt");
                if (File.Exists(fileUrlPath))
                {
                    var url = File.ReadAllText(fileUrlPath).Trim();
                    url = HttpContext.Current.Request.Url.AbsoluteUri + string.Format(url, context.SessionId);
                    Response.Redirect(url);
                }
            }
        }

        /// <summary>
        /// MVC appliction start event
        /// </summary>
        protected void Application_Start()
        {
            // Register areas and routes
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            // Register global filters
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Register scripts and css bundles
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Register unity configuration
            BootstrapConfig.Register();

            //Register providers
            ProvidersConfig.Register();

            // Register custom flag enum model binder
            ModelBinders.Binders.DefaultBinder = new CustomModelBinder();

            AsyncManagerConfig.Register();
        }

        /// <summary>
        /// Log the application error
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            if (!string.IsNullOrEmpty(exception.Message) &&
                exception.Message.Contains("SSONotify"))
            {
                //No need to log this SSONotify error as it is a known issue
                return;
            }

            Logger.Error(LoggingConstants.ApplicationError, LoggingConstants.ModuleGlobal, null, exception);

            var context = HttpContext.Current.Items["Context"] as Context;
            Response.Redirect(null != context && !string.IsNullOrEmpty(context.TenantAlias)
                ? string.Format(@"~\{0}\Core\Error", context.TenantAlias)
                : @"~\Core\Error");
        }

        /// <summary>
        /// Event triggered when Session expires/ends
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Session_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// MVC application end event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_End(object sender, EventArgs e)
        {
            CommonService.ClearSessionLogs();
        }
    }
}