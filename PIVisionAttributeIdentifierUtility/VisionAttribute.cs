using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF.Asset;
using OSIsoft.AF;
using OSIsoft.AF.Search;

namespace PIVisionAttributeIdentifierUtility
{
    class VisionAttribute
    {
        public void SearchAndPrint(AFDatabase myDB, string guid)
        {

            IList<AFSearchTokenBase> mySearchTokens = new List<AFSearchTokenBase>() { };
            mySearchTokens.Add(new AFSearchFilterToken(AFSearchFilter.Name, "*"));
            mySearchTokens.Add(new AFSearchFilterToken(AFSearchFilter.Element, null));
            AFAttributeSearch mySearch = new AFAttributeSearch(myDB, "mySearch", mySearchTokens);
            IEnumerable<AFAttribute> myAttrs2 = mySearch.FindObjects();
         
            foreach (AFAttribute item in myAttrs2)
            {
                if (item.ID == new Guid(guid))

                    Console.WriteLine("Name: "+item.Name + " | DR: " + item.DataReferencePlugIn + " | path: " + item.GetPath() );
            }
        }

        public AFAttribute SearchAndPrint2(PISystem afserver, Guid ElementGUID, Guid AttributeGUID)
        {
            AFElement afelement = AFElement.FindElement(afserver, ElementGUID);
            AFAttribute afattribute = AFAttribute.FindAttribute(afelement, AttributeGUID);
            return afattribute;
        }
    }
}
