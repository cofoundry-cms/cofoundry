using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Domain
{
    public class YouTubeVideo
    {
        public long? Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Duration { get; set; }

        public int? Height { get; set; }

        public DateTime? UploadDate { get; set; }

        public int? Width { get; set; }
    }
}