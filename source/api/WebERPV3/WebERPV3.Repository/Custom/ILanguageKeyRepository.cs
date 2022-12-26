using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static WebERPV3.Common.Enums;

namespace WebERPV3.Repository
{
    public interface ILanguageKeyRepository : IBase<LanguageKey>
    {
        public Result<object> UploadI18nDictionaries(string path, ServerDirectoryType serverDirectoryType= ServerDirectoryType.Folder);
    }
}
