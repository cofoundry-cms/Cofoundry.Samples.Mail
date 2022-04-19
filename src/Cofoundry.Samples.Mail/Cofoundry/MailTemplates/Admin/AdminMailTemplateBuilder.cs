﻿using Cofoundry.Core.Mail;
using Cofoundry.Core.Web;
using Cofoundry.Domain;
using Cofoundry.Domain.MailTemplates;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Samples.Mail.AdminMailTemplates
{
    /// <summary>
    /// To override the built-in admin mail templates we need to
    /// implement the <see cref="IUserMailTemplateBuilder{T}"/> interface for the
    /// Cofoundry admin user area. DI will pick up your custom
    /// builder automatically.
    /// 
    /// This example shows the various levels of customizations you
    /// can make to the default templates.
    /// </summary>
    public class AdminMailTemplateBuilder : IUserMailTemplateBuilder<CofoundryAdminUserArea>
    {
        const string LAYOUT_PATH = "~/Cofoundry/MailTemplates/Admin/Layouts/_MailLayout";
        private readonly ISiteUrlResolver _siteUrlResolver;

        public AdminMailTemplateBuilder(
            ISiteUrlResolver siteUrlResolver
            )
        {
            _siteUrlResolver = siteUrlResolver;
        }

        /// <summary>
        /// This example simply customizes the layout file, which can be useful 
        /// for wrapping the default content with your own branding.
        /// </summary>
        public async Task<IMailTemplate> BuildNewUserWithTemporaryPasswordTemplateAsync(INewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            // build the default template so we can modify any properties we want to customize
            var template = await context.BuildDefaultTemplateAsync();

            // You can customize the layout file used by the default template by 
            // changing the LayoutFile property. An app relative path is required because 
            // the mail template folder isn't registered as a view location. This 
            // avoids unexpected conflicts.
            template.LayoutFile = LAYOUT_PATH;

            return template;
        }

        /// <summary>
        /// This example shows you how to change the email subject.
        /// </summary>
        public async Task<IMailTemplate> BuildPasswordChangedTemplateAsync(IPasswordChangedTemplateBuilderContext context)
        {
            // build the default template
            var template = await context.BuildDefaultTemplateAsync();

            // customize the layout file
            template.LayoutFile = LAYOUT_PATH;

            // customize the subject, the optional {0} token is replaced with the application name
            template.SubjectFormat = "{0}: You've changed your password!";

            return template;
        }

        /// <summary>
        /// In this example, the view file is customized, which is useful if
        /// you want to change the wording of the email, but don't need any
        /// additional properties in the template model
        /// </summary>
        public async Task<IMailTemplate> BuildPasswordResetTemplateAsync(IPasswordResetTemplateBuilderContext context)
        {
            // build the default template
            var template = await context.BuildDefaultTemplateAsync();

            // customize the layout file
            template.LayoutFile = LAYOUT_PATH;

            // customize the subject, the {0} token is optional
            template.SubjectFormat = "Your password has been reset!";

            // customize the view file
            template.ViewFile = "~/Cofoundry/MailTemplates/Admin/Templates/PasswordResetMailTemplate";

            return template;
        }

        public Task<IMailTemplate> BuildAccountVerificationTemplateAsync(IAccountVerificationTemplateBuilderContext context)
        {
            // Admin site does not use an account verification flow
            throw new NotSupportedException();
        }

        /// <summary>
        /// This example completely customizes the template model and the 
        /// view file, adding a "first name" property. The method only requires 
        /// that you return an <see cref="IMailTemplate"/> instance, so you are
        /// free to customize the process as little or as much as you like.
        /// </summary>
        public Task<IMailTemplate> BuildAccountRecoveryTemplateAsync(IAccountRecoveryTemplateBuilderContext context)
        {
            // build a custom template instance, adding in any custom data
            var template = new AccountRecoveryMailTemplate()
            {
                DisplayName = context.User.DisplayName,
                RecoveryUrl = _siteUrlResolver.MakeAbsolute(context.RecoveryUrlPath),
                Username = context.User.Username
            };

            return Task.FromResult<IMailTemplate>(template);
        }
    }
}
