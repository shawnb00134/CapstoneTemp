using CAMCMSServer.Model;
using CAMCMSServer.Model.JsonCacheModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CAMCMSServer.Utils
{
    public class GenerateCachedModule
    {
        /// <summary>
        /// Generates the cache module for a published module
        /// 
        /// It will create a CachedModule object that will generate pages of element sets and their elements, the module's title,
        /// a list of all the elementSets for easy access to any elementSet that is needed
        /// and anchors to generate the table of contents for the module from the anchor elements.
        ///
        /// 
        /// </summary>
        /// <param name="module">The module that contains all elementSets and elements in the module </param>
        /// <returns>The Json of the cached Module</returns>
        public static string GenerateCacheModule(Module module)
        {
            var anchorElementId = 7; // This is the anchor element type id, possibly make ENUMS or something to make it more clear

            var moduleJson = new CachedModule()
            {
                pages = new List<List<CacheElementSet>>(),
                title = module.Title,
                anchors = new List<CacheAnchor>(),
                elementSets = new List<CacheElementSet>()
            };
            var currentPage = 0;
            moduleJson.pages.Add(new List<CacheElementSet>()); // Initialize first page

            CacheAnchor lastParentNode = null;

            foreach (var elementSet in module.ElementSets)
            {
                var newElementSet = new CacheElementSet()
                {
                    locations = new List<CacheLocation>(),
                    place = (int)elementSet.Place,
                    styling = JObject.Parse(elementSet.StylingJson),
                    setLocationId = (int)elementSet.SetLocationId
                };

                foreach (var element in elementSet.Elements)
                {
                    var newLocation = new CacheLocation()
                    {
                        place = element.Place,
                        element = element.Element,
                        locationAttribute = JObject.Parse(element.LocationAttributeJson)
                    };

                    if (newLocation.element.TypeId == anchorElementId)
                    {
                        var anchor = new CacheAnchor()
                        {
                            anchorId = newLocation.element.ElementId ?? 0,
                            content = newLocation.element.Title,
                            subHeader = false,
                            level = element.Attributes.HeadingLevel ?? 0,
                            headerChildren = new List<CacheAnchor>(),
                            headerParent = null
                        };

                        if (lastParentNode == null || lastParentNode.level == anchor.level)
                        {
                            lastParentNode = anchor;
                            moduleJson.anchors.Add(anchor);
                        }
                        else if (lastParentNode.level < anchor.level)
                        {
                            if (lastParentNode.headerChildren.Count == 0 || lastParentNode.level + 1 == anchor.level)
                            {
                                anchor.headerParent = lastParentNode;
                                lastParentNode.headerChildren.Add(anchor);
                            }
                            else
                            {
                                addAnchor(lastParentNode, anchor); 
                            }
                        }

                        if (newElementSet.styling["has_page_break"] != null && newElementSet.styling["has_page_break"].ToString() == "before")
                        {
                            currentPage++;
                            moduleJson.pages.Add(new List<CacheElementSet>()); 
                        }

                        anchor.page = currentPage;
                        moduleJson.anchors.Add(anchor);
                    }
                    
                    newElementSet.locations.Add(newLocation);
                }

                moduleJson.pages[currentPage].Add(newElementSet);

                if (newElementSet.styling["has_page_break"] != null && newElementSet.styling["has_page_break"].ToString() == "after")
                {
                    currentPage++;
                    moduleJson.pages.Add(new List<CacheElementSet>()); 
                }
                moduleJson.elementSets.Add(newElementSet);
            }

            return JsonConvert.SerializeObject(moduleJson); 
        }

        private static void addAnchor(CacheAnchor node, CacheAnchor child )
        {
            if (node.headerChildren.Count == 0)
            {
                child.headerParent = node;
                node.headerChildren.Add(child);
            }

            if (node == null)
            {
                return;
            }

            while (node.headerChildren.Count != 0)
            {
                node = node.headerChildren[node.headerChildren.Count - 1];
                if ((node.level + 1) == child.level)
                {
                    child.headerParent = node;
                    node.headerChildren.Add(child);
                    return;
                }
            }
            node.headerChildren.Add(child);
        }
    }
}
