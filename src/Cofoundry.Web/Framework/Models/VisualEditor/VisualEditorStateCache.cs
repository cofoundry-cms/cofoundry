using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    public class VisualEditorStateCache : IVisualEditorStateCache
    {
        const string CACHE_KEY = "Cofoundry.Web.VisualEditorStateCache";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VisualEditorStateCache(
            IHttpContextAccessor httpContextAccessor
            )
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Sets the cache value.
        /// </summary>
        public VisualEditorState Get()
        {
            var cache = _httpContextAccessor.HttpContext?.Items;
            if (cache == null) return null;

            return cache[CACHE_KEY] as VisualEditorState;
        }

        /// <summary>
        /// Gets the cache value or null if it has not been set.
        /// </summary>
        public void Set(VisualEditorState data)
        {
            var cache = _httpContextAccessor.HttpContext?.Items;

            if (cache == null)
            {
                throw new InvalidOperationException("Cannot set the cache outside of a request.");
            }

            cache[CACHE_KEY] = data;
        }
    }
}
