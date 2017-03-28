//using RazorEngine.Templating;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Hosting;

//namespace Cofoundry.Core.Mail
//{
//    public class VirtualPathTemplateManager : ITemplateManager
//    {
//        private readonly IViewFileReader _viewFileReader;

//        public VirtualPathTemplateManager(
//            IViewFileReader viewFileReader
//            )
//        {
//            _viewFileReader = viewFileReader;
//        }

//        public ITemplateSource Resolve(ITemplateKey key)
//        {
//            var templateKey = key as NameOnlyTemplateKey;
//            if (templateKey == null)
//            {
//                throw new NotSupportedException("You can only use NameOnlyTemplateKey with this manager");
//            }

//            var template = _viewFileReader.Read(templateKey.Name);

//            return new LoadedTemplateSource(template, templateKey.Name);
//        }

//        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
//        {
//            return new NameOnlyTemplateKey(name, resolveType, context);
//        }

//        /// <summary>
//        /// Throws NotSupportedException.
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="source"></param>
//        public void AddDynamic(ITemplateKey key, ITemplateSource source)
//        {
//            throw new NotSupportedException("Adding templates dynamically is not supported! Instead you probably want to use the full-path in the name parameter?");
//        }
//    }
//}
