using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Aurum.Security
{
    /// <summary>
    /// Поставщик сложного правила безопасности
    /// </summary>
    public class ComplexRuleProvider : ISecurityRuleProvider
    {
        private ComplexRule complexRule;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dic">Справочник метаданных</param>
        /// <param name="securities">Системы</param>
        public ComplexRuleProvider(XPDictionary dic, ISelectDataSecurity[] securities)
        {
            ISecurityRule[] rules = securities.Select(security =>
                SecurityStrategy.TraceLevel != TraceLevel.Off ?
                new SecurityRuleLogger(dic, security, new FilterLogger(Logger.ConvertToLogLevel(SecurityStrategy.TraceLevel), Logger.Instance)) :
                new SecurityRule(dic, security)).ToArray();
            complexRule = new ComplexRule(rules);
        }

        /// <contentfrom cref="ISecurityRuleProvider.GetRule" />
        public ISecurityRule GetRule(XPClassInfo classInfo)
        {
            return complexRule;
        }
    }

    /// <summary>
    /// Сложное правило безопасности операций с данными
    /// </summary>
    /// <remarks>Сложное правило безопасности вычисляет пересечение результатов простых правил</remarks>
    public class ComplexRule : ISecurityRule
    {
        private ISecurityRule[] rules;
        private XPClassInfo[] supportedObjectTypes;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="rules">Простые правила безопасности</param>
        public ComplexRule(ISecurityRule[] rules)
        {
            this.rules = rules;
            this.supportedObjectTypes = null;
        }

        /// <contentfrom cref="ISecurityRule.GetSelectFilterCriteria" />
        public bool GetSelectFilterCriteria(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, out CriteriaOperator criteria)
        {
            List<CriteriaOperator> subList = new List<CriteriaOperator>();
            CriteriaOperator subCriteria;
            foreach (ISecurityRule rule in rules)
                if (rule.GetSelectFilterCriteria(context, classInfo, out subCriteria))
                    subList.Add(subCriteria);
            if (subList.Count == 0)
                criteria = null;
            else if (subList.Count == 1)
                criteria = subList[0];
            else
                criteria = new GroupOperator(GroupOperatorType.And, subList.ToArray());
            return subList.Count > 0;
        }

        /// <contentfrom cref="ISecurityRule.GetSelectMemberExpression" />
        public bool GetSelectMemberExpression(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, XPMemberInfo memberInfo, out CriteriaOperator expression)
        {
            CriteriaOperator subExpression;
            expression = null;
            foreach (ISecurityRule rule in rules)
                if (rule.GetSelectMemberExpression(context, classInfo, memberInfo, out subExpression))
                {
                    if (ReferenceEquals(expression, null))
                        expression = subExpression;
                    else if (!CriteriaOperator.CriterionEquals(expression, subExpression))
                    {
                        expression = new OperandValue(null);
                        return true;
                    }
                }
            return !ReferenceEquals(expression, null);
        }

        /// <contentfrom cref="ISecurityRule.SupportedObjectTypes" />
        public XPClassInfo[] SupportedObjectTypes
        {
            get 
            {
                if (supportedObjectTypes == null)
                {
                    List<XPClassInfo> list = null;
                    foreach (ISecurityRule rule in rules)
                        if (list == null)
                            list = new List<XPClassInfo>(rule.SupportedObjectTypes);
                        else
                            list = new List<XPClassInfo>(list.Intersect(rule.SupportedObjectTypes));
                    supportedObjectTypes = list != null ? list.ToArray() : new XPClassInfo[0];
                }
                return supportedObjectTypes;
            }
        }

        /// <contentfrom cref="ISecurityRule.ValidateMemberOnSave" />
        public ValidateMemberOnSaveResult ValidateMemberOnSave(DevExpress.Xpo.SecurityContext context, XPMemberInfo memberInfo, object theObject, object realObjectOnLoad, object value, object valueOnLoad, object realValueOnLoad)
        {
            ValidateMemberOnSaveResult result = ValidateMemberOnSaveResult.DoSaveMember, subResult;
            foreach (ISecurityRule rule in rules)
            {
                if (result == ValidateMemberOnSaveResult.DoRaiseException) break;
                subResult = rule.ValidateMemberOnSave(context, memberInfo, theObject, realObjectOnLoad, value, valueOnLoad, realValueOnLoad);
                switch (subResult)
                {
                    case ValidateMemberOnSaveResult.DoRaiseException: result = subResult; break;
                    case ValidateMemberOnSaveResult.DoNotSaveMember: 
                        if (result == ValidateMemberOnSaveResult.DoSaveMember) result = subResult; break;
                }
            }
            return result;
        }

        /// <contentfrom cref="ISecurityRule.ValidateObjectOnDelete" />
        public bool ValidateObjectOnDelete(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, object theObject, object realObjectOnLoad)
        {
            bool result = true;
            foreach (ISecurityRule rule in rules)
                result = result && rule.ValidateObjectOnDelete(context, classInfo, theObject, realObjectOnLoad);
            return result;
        }

        /// <contentfrom cref="ISecurityRule.ValidateObjectOnSave" />
        public bool ValidateObjectOnSave(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, object theObject, object realObjectOnLoad)
        {
            bool result = true;
            foreach (ISecurityRule rule in rules)
                result = result && rule.ValidateObjectOnSave(context, classInfo, theObject, realObjectOnLoad);
            return result;
        }

        /// <contentfrom cref="ISecurityRule.ValidateObjectOnSelect" />
        public bool ValidateObjectOnSelect(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, object realObjectOnLoad)
        {
            bool result = true;
            foreach (ISecurityRule rule in rules)
                result = result && rule.ValidateObjectOnSelect(context, classInfo, realObjectOnLoad);
            return result;
        }
    }
}
