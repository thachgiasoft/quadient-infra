using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.ModelBinding;
using System.Web.Routing;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.HtmlControls;

namespace Infrastructure.WebFramework.Fakes
{
    public interface IPage
    {

        HttpApplicationState Application { get; }
        TimeSpan AsyncTimeout { get; set; }
        Control AutoPostBackControl { get; set; }
        bool Buffer { get; set; }
        Cache Cache { get; }
        string ClientQueryString { get; }
        ClientScriptManager ClientScript { get; }
        string ClientTarget { get; set; }
        int CodePage { get; set; }
        string ContentType { get; set; }
        string Culture { get; set; }
        bool EnableEventValidation { get; set; }
        bool EnableViewState { get; set; }
        bool EnableViewStateMac { get; set; }
        string ErrorPage { get; set; }
        HtmlForm Form { get; }
        string ID { get; set; }
        char IdSeparator { get; }
        bool IsAsync { get; }
        bool IsCallback { get; }
        bool IsCrossPagePostBack { get; }
        bool IsPostBack { get; }
        bool IsPostBackEventControlRegistered { get; }
        bool IsReusable { get; }
        bool IsValid { get; }
        IDictionary Items { get; }
        int LCID { get; set; }
        bool MaintainScrollPositionOnPostBack { get; set; }
        MasterPage Master { get; }
        string MasterPageFile { get; set; }
        int MaxPageStateFieldLength { get; set; }
        string MetaDescription { get; set; }
        string MetaKeywords { get; set; }
        ModelBindingExecutionContext ModelBindingExecutionContext { get; }
        ModelStateDictionary ModelState { get; }
        PageAdapter PageAdapter { get; }
        Page PreviousPage { get; }
        HttpRequest Request { get; }
        HttpResponse Response { get; }
        string ResponseEncoding { get; set; }
        RouteData RouteData { get; }
        HttpServerUtility Server { get; }
        HttpSessionState Session { get; }
        bool SkipFormActionValidation { get; set; }
        string StyleSheetTheme { get; set; }
        string Theme { get; set; }
        string Title { get; set; }
        TraceContext Trace { get; }
        bool TraceEnabled { get; set; }
        TraceMode TraceModeValue { get; set; }
        string UICulture { get; set; }
        UnobtrusiveValidationMode UnobtrusiveValidationMode { get; set; }
        IPrincipal User { get; }
        ValidateRequestMode ValidateRequestMode { get; set; }
        ValidatorCollection Validators { get; }
        ViewStateEncryptionMode ViewStateEncryptionMode { get; set; }
        string ViewStateUserKey { get; set; }
        bool Visible { get; set; }
        void AddOnPreRenderCompleteAsync(BeginEventHandler beginHandler, EndEventHandler endHandler);
        void AddOnPreRenderCompleteAsync(BeginEventHandler beginHandler, EndEventHandler endHandler, object state);
        void DesignerInitialize();
        void ExecuteRegisteredAsyncTasks();
        Control FindControl(string id);
        object GetDataItem();
        int GetTypeHashCode();
        ValidatorCollection GetValidators(string validationGroup);
        bool IsStartupScriptRegistered(string key);
        string MapPath(string virtualPath);
        void ProcessRequest(HttpContext context);
        void RegisterArrayDeclaration(string arrayName, string arrayValue);
        void RegisterAsyncTask(PageAsyncTask task);
        void RegisterHiddenField(string hiddenFieldName, string hiddenFieldInitialValue);
        void RegisterOnSubmitStatement(string key, string script);
        void RegisterRequiresControlState(Control control);
        void RegisterRequiresPostBack(Control control);
        void RegisterRequiresRaiseEvent(IPostBackEventHandler control);
        void RegisterRequiresViewStateEncryption();
        void RegisterViewStateHandler();
        bool RequiresControlState(Control control);
        void SetFocus(Control control);
        void SetFocus(string clientID);
        bool TryUpdateModel<TModel>(TModel model) where TModel : class;
        bool TryUpdateModel<TModel>(TModel model, IValueProvider valueProvider) where TModel : class;
        void UnregisterRequiresControlState(Control control);
        void UpdateModel<TModel>(TModel model) where TModel : class;
        void UpdateModel<TModel>(TModel model, IValueProvider valueProvider) where TModel : class;
        void Validate();
        void Validate(string validationGroup);
        void VerifyRenderingInServerForm(Control control);
    }
}
