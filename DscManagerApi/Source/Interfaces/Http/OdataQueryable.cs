// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OdataQueryable.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Http
{
    // TODO Fix this
    /*
    public class CustomQueryableAttribute : EnableQueryAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            long? originalsize = null;

            object responseObject;
            actionExecutedContext.Response.TryGetContentValue(out responseObject);
            var originalquery = responseObject as IQueryable<object>;

            //if (originalquery != null && inlinecount == "allpages")
            //    originalsize = originalquery.Count();

            base.OnActionExecuted(actionExecutedContext);

            if (ResponseIsValid(actionExecutedContext.Response))
            {
                actionExecutedContext.Response.TryGetContentValue(out responseObject);

                if (responseObject is IQueryable)
                {
                    var robj = responseObject as IQueryable<NodeView>;

                    if (robj != null)
                    {
                        //actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, new ODataMetadata<object>(robj, originalsize));
                        actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                            HttpStatusCode.OK,
                            new Test { Nodes = robj });
                    }
                }
            }
        }

        private bool ResponseIsValid(HttpResponseMessage response)
        {
            if (response == null || response.StatusCode != HttpStatusCode.OK || !(response.Content is ObjectContent)) return false;
            return true;
        }
    }

    public class Test
    {
        public IEnumerable<NodeView> Nodes { get; set; }

        public int Count { get; set; } = 5;
    }*/
}