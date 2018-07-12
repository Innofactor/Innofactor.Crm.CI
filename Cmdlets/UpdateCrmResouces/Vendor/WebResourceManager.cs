// PROJECT : MsCrmTools.WebResourcesManager
// This project was developed by Tanguy Touzard
// CODEPLEX: http://xrmtoolbox.codeplex.com
// BLOG: http://mscrmtools.blogspot.com

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Innofactor.Crm.CI.Cmdlets.Vendor
{
    /// <summary>
    /// Class that manages action on web resources
    /// in Microsoft Dynamics CRM 2011
    /// </summary>
    internal static class WebResourceManager
    {
        internal static string GetBase64StringFromString(string content)
        {
            var byt = System.Text.Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(byt);
        }

        internal static string GetContentFromBase64String(string base64)
        {
            var b = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(b);
        }

        internal static void AddToSolution(this CrmServiceClient service, EntityCollection resources, string solutionUniqueName)
        {
            foreach (var resource in resources.Entities)
            {
                var request = new AddSolutionComponentRequest
                {
                    AddRequiredComponents = false,
                    ComponentId = resource.Id,
                    ComponentType = SolutionComponentType.WebResource,
                    SolutionUniqueName = solutionUniqueName
                };

                service.Execute(request);
            }
        }

        /// <summary>
        /// Creates the provided web resource
        /// </summary>
        /// <param name="webResource">Web resource to create</param>
        internal static Guid CreateWebResource(this CrmServiceClient service, Entity webResource)
        {
            try
            {
                return service.Create(webResource);
            }
            catch (Exception error)
            {
                throw new Exception("Error while creating web resource: " + error.Message);
            }
        }

        /// <summary>
        /// Deletes the provided web resource
        /// </summary>
        /// <param name="webResource">Web resource to delete</param>
        internal static void DeleteWebResource(this CrmServiceClient service, Entity webResource)
        {
            try
            {
                service.Delete(webResource.LogicalName, webResource.Id);
            }
            catch (Exception error)
            {
                throw new Exception("Error while deleting web resource: " + error.Message);
            }
        }

        internal static bool HasDependencies(this CrmServiceClient service, Guid webresourceId)
        {
            var request = new RetrieveDependenciesForDeleteRequest
            {
                ComponentType = SolutionComponentType.WebResource,
                ObjectId = webresourceId
            };

            var response = (RetrieveDependenciesForDeleteResponse)service.Execute(request);
            return response.EntityCollection.Entities.Count != 0;
        }

        internal static void PublishWebResources(this CrmServiceClient service, EntityCollection resources)
        {
            try
            {
                var idsXml = string.Empty;

                foreach (var resource in resources.Entities)
                {
                    idsXml += string.Format("<webresource>{0}</webresource>", resource.Id.ToString("B"));
                }

                var pxReq1 = new PublishXmlRequest
                {
                    ParameterXml = string.Format("<importexportxml><webresources>{0}</webresources></importexportxml>", idsXml)
                };

                service.Execute(pxReq1);
            }
            catch (Exception error)
            {
                throw new Exception("Error while publishing web resources: " + error.Message);
            }
        }

        /// <summary>
        /// Retrieves a specific web resource from its unique identifier
        /// </summary>
        /// <param name="webresourceId">Web resource unique identifier</param>
        /// <returns>Web resource</returns>
        internal static Entity RetrieveWebResource(this CrmServiceClient service, Guid webresourceId)
        {
            try
            {
                return service.Retrieve("webresource", webresourceId, new ColumnSet(true));
            }
            catch (Exception error)
            {
                throw new Exception("Error while retrieving web resource: " + error.Message);
            }
        }

        /// <summary>
        /// Retrieves a specific web resource from its unique name
        /// </summary>
        /// <param name="name">Web resource unique name</param>
        /// <returns>Web resource</returns>
        internal static Entity RetrieveWebResource(this CrmServiceClient service, string name)
        {
            try
            {
                var qba = new QueryByAttribute("webresource");
                qba.Attributes.Add("name");
                qba.Values.Add(name);
                qba.ColumnSet = new ColumnSet(true);

                var collection = service.RetrieveMultiple(qba);

                if (collection.Entities.Count > 1)
                {
                    throw new Exception(string.Format("there are more than one web resource with name '{0}'", name));
                }

                return collection.Entities.FirstOrDefault();
            }
            catch (Exception error)
            {
                throw new Exception("Error while retrieving web resource: " + error.Message);
            }
        }

        /// <summary>
        /// Retrieves all web resources that are customizable
        /// </summary>
        /// <returns>List of web resources</returns>
        internal static EntityCollection RetrieveWebResources(this CrmServiceClient service, Guid solutionId, List<int> types, bool hideMicrosoftWebresources)
        {
            try
            {
                if (solutionId == Guid.Empty)
                {
                    var qe = new QueryExpression("webresource")
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression
                        {
                            Filters =
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("ishidden", ConditionOperator.Equal, false),
                                    }
                                },
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.Or,
                                     Conditions =
                                    {
                                        new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                                        new ConditionExpression("iscustomizable", ConditionOperator.Equal, true),
                                    }
                                }
                            }
                        },
                        Orders = { new OrderExpression("name", OrderType.Ascending) }
                    };

                    if (hideMicrosoftWebresources)
                    {
                        qe.Criteria.Filters.First().Conditions.AddRange(
                            new ConditionExpression("name", ConditionOperator.DoesNotBeginWith, "cc_MscrmControls"),
                            new ConditionExpression("name", ConditionOperator.DoesNotBeginWith, "msdyn_")
                            );
                    }

                    if (types.Count != 0)
                    {
                        qe.Criteria.Filters.First().Conditions.Add(new ConditionExpression("webresourcetype", ConditionOperator.In, types.ToArray()));
                    }

                    return service.RetrieveMultiple(qe);
                }
                else
                {
                    var qba = new QueryByAttribute("solutioncomponent")
                    {
                        ColumnSet = new ColumnSet(true)
                    };
                    qba.Attributes.AddRange(new[] { "solutionid", "componenttype" });
                    qba.Values.AddRange(new object[] { solutionId, 61 });

                    var components = service.RetrieveMultiple(qba);

                    var list = components.Entities
                        .Select(x => x.Attributes["objectid"])
                        .Where(x => x != null)
                        .Select(x => ((Guid)x).ToString("B")).ToList();

                    if (list.Count > 0)
                    {
                        var qe = new QueryExpression("webresource")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression
                            {
                                Filters =
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression("ishidden", ConditionOperator.Equal, false),
                                        new ConditionExpression("webresourceid", ConditionOperator.In, list.ToArray()),
                                    }
                                },
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.Or,
                                     Conditions =
                                    {
                                        new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                                        new ConditionExpression("iscustomizable", ConditionOperator.Equal, true),
                                    }
                                }
                            }
                            },
                            Orders = { new OrderExpression("name", OrderType.Ascending) }
                        };

                        if (types.Count != 0)
                        {
                            qe.Criteria.Filters.First().Conditions.Add(new ConditionExpression("webresourcetype", ConditionOperator.In, types.ToArray()));
                        }

                        return service.RetrieveMultiple(qe);
                    }

                    return new EntityCollection();
                }
            }
            catch (Exception error)
            {
                throw new Exception("Error while retrieving web resources: " + error.Message);
            }
        }

        /// <summary>
        /// Updates the provided web resource
        /// </summary>
        /// <param name="wr">Web resource to update</param>
        internal static void UpdateWebResource(this CrmServiceClient service, Entity wr)
        {
            try
            {
                var script = wr;

                if (!script.Contains("webresourceid"))
                {
                    var existingEntity = service.RetrieveWebResource(script.Attributes["name"] as string);

                    if (existingEntity == null)
                    {
                        script.Id = service.CreateWebResource(script);
                    }
                    else
                    {
                        script.Id = existingEntity.Id;

                        if (!script.Contains("displayname") && existingEntity.Contains("displayname"))
                        {
                            script.Attributes.Add("displayname", existingEntity.Attributes["displayname"]);
                        }

                        if (!script.Contains("description") && existingEntity.Contains("description"))
                        {
                            script.Attributes.Add("description", existingEntity.Attributes["description"]);
                        }

                        service.Update(script);
                    }
                }
                else
                {
                    service.Update(script);
                }
            }
            catch (Exception error)
            {
                throw new Exception("Error while updating web resource: " + error.Message);
            }
        }
    }
}