﻿using System.Collections.Generic;
using P7.AspNetCore.Authentication.OAuth;

namespace P7.AspNetCore.Authentication.Google
{
    public class GoogleChallengeProperties : OAuthChallengeProperties
    {
        /// <summary>
        /// The parameter key for the "access_type" argument being used for a challenge request.
        /// </summary>
        public static readonly string AccessTypeKey = "access_type";

        /// <summary>
        /// The parameter key for the "approval_prompt" argument being used for a challenge request.
        /// </summary>
        public static readonly string ApprovalPromptKey = "approval_prompt";

        /// <summary>
        /// The parameter key for the "include_granted_scopes" argument being used for a challenge request.
        /// </summary>
        public static readonly string IncludeGrantedScopesKey = "include_granted_scopes";

        /// <summary>
        /// The parameter key for the "login_hint" argument being used for a challenge request.
        /// </summary>
        public static readonly string LoginHintKey = "login_hint";

        /// <summary>
        /// The parameter key for the "prompt" argument being used for a challenge request.
        /// </summary>
        public static readonly string PromptParameterKey = "prompt";

        public GoogleChallengeProperties()
        { }

        public GoogleChallengeProperties(IDictionary<string, string> items)
            : base(items)
        { }

        public GoogleChallengeProperties(IDictionary<string, string> items, IDictionary<string, object> parameters)
            : base(items, parameters)
        { }

        /// <summary>
        /// The "access_type" parameter value being used for a challenge request.
        /// </summary>
        public string AccessType
        {
            get => GetParameter<string>(AccessTypeKey);
            set => SetParameter(AccessTypeKey, value);
        }

        /// <summary>
        /// The "approval_prompt" parameter value being used for a challenge request.
        /// </summary>
        public string ApprovalPrompt
        {
            get => GetParameter<string>(ApprovalPromptKey);
            set => SetParameter(ApprovalPromptKey, value);
        }

        /// <summary>
        /// The "include_granted_scopes" parameter value being used for a challenge request.
        /// </summary>
        public bool? IncludeGrantedScopes
        {
            get => GetParameter<bool?>(IncludeGrantedScopesKey);
            set => SetParameter(IncludeGrantedScopesKey, value);
        }

        /// <summary>
        /// The "login_hint" parameter value being used for a challenge request.
        /// </summary>
        public string LoginHint
        {
            get => GetParameter<string>(LoginHintKey);
            set => SetParameter(LoginHintKey, value);
        }

        /// <summary>
        /// The "prompt" parameter value being used for a challenge request.
        /// </summary>
        public string Prompt
        {
            get => GetParameter<string>(PromptParameterKey);
            set => SetParameter(PromptParameterKey, value);
        }
    }
}
