using Microsoft.EntityFrameworkCore.Storage;
using Sio.Cms.Lib.Models.Cms;
using Sio.Cms.Lib.Repositories;
using Sio.Cms.Lib.Services;
using Sio.Common.Helper;
using Sio.Domain.Core.ViewModels;
using Sio.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sio.Cms.Lib.ViewModels.SioTemplates
{
    public class UpdateViewModel
      : ViewModelBase<SioCmsContext, SioTemplate, UpdateViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("themeId")]
        public int ThemeId { get; set; }

        [JsonProperty("themeName")]
        public string ThemeName { get; set; }

        [JsonProperty("folderType")]
        public string FolderType { get; set; }

        [JsonProperty("fileFolder")]
        public string FileFolder { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [Required]
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("mobileContent")]
        public string MobileContent { get; set; } = "{}";

        [JsonProperty("spaContent")]
        public string SpaContent { get; set; } = "";

        [JsonProperty("scripts")]
        public string Scripts { get; set; }

        [JsonProperty("styles")]
        public string Styles { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("lastModified")]
        public DateTime? LastModified { get; set; }

        [JsonProperty("modifiedBy")]
        public string ModifiedBy { get; set; }

        #endregion Models

        #region Views

        [JsonProperty("layout")]
        public string Layout { get; set; }

        [JsonProperty("assetFolder")]
        public string AssetFolder
        {
            get
            {
                return CommonHelper.GetFullPath(new string[] {
                    SioConstants.Folder.FileFolder,
                    SioConstants.Folder.TemplatesAssetFolder,
                     SeoHelper.GetSEOString(ThemeName) });
            }
        }

        [JsonProperty("templateFolder")]
        public string TemplateFolder
        {
            get
            {
                return CommonHelper.GetFullPath(new string[] {
                    SioConstants.Folder.TemplatesFolder, SeoHelper.GetSEOString(ThemeName) });
            }
        }

        [JsonProperty("templatePath")]
        public string TemplatePath
        {
            get
            {
                return $"/{FileFolder}/{FileName}{Extension}";
            }
        }


        #endregion Views

        #endregion Properties

        #region Contructors

        public UpdateViewModel()
            : base()
        {
        }

        public UpdateViewModel(SioTemplate model, SioCmsContext _context = null, IDbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Overrides

        #region Common

        public override void ExpandView(SioCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var file = FileRepository.Instance.GetFile(FileName, Extension, FileFolder);
            if (!string.IsNullOrWhiteSpace(file?.Content))
            {
                Content = file.Content;
            }
        }

        public override void Validate(SioCmsContext _context, IDbContextTransaction _transaction)
        {
            base.Validate(_context, _transaction);
            if (IsValid)
            {
                if (Id == 0)
                {
                    if (_context.SioTemplate.Any(t => t.FileName == FileName && t.FolderType == FolderType && t.ThemeId == ThemeId))
                    {
                        FileName = $"{FileName}_1";
                    }
                }
            }
        }

        public override SioTemplate ParseModel(SioCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            if (Id == 0)
            {
                Id = Repository.Max(m => m.Id, _context,  _transaction).Data + 1;
                CreatedDateTime = DateTime.UtcNow;
            }
            FileFolder = CommonHelper.GetFullPath(new string[]
                {
                    SioConstants.Folder.TemplatesFolder
                    , ThemeName
                    , FolderType
                });

            Content = Content?.Trim();
            Scripts = Scripts?.Trim();
            Styles = Styles?.Trim();
            return base.ParseModel(_context, _transaction);
        }

        #endregion Common

        #region Async

        public override RepositoryResponse<SioTemplate> RemoveModel(bool isRemoveRelatedModels = false, SioCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result = base.RemoveModel(isRemoveRelatedModels, _context, _transaction);
            if (result.IsSucceed)
            {
                TemplateRepository.Instance.DeleteTemplate(FileName, FileFolder);
            }
            return result;
        }

        public override RepositoryResponse<bool> SaveSubModels(SioTemplate parent, SioCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            TemplateRepository.Instance.SaveTemplate(new TemplateViewModel()
            {
                Filename = FileName,
                Extension = Extension,
                Content = Content,
                FileFolder = FileFolder
            });
            return base.SaveSubModels(parent, _context, _transaction);
        }

        #endregion Async

        #region Async

        public override async Task<RepositoryResponse<SioTemplate>> RemoveModelAsync(bool isRemoveRelatedModels = false, SioCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result = await base.RemoveModelAsync(isRemoveRelatedModels, _context, _transaction);
            if (result.IsSucceed)
            {
                TemplateRepository.Instance.DeleteTemplate(FileName, FileFolder);
            }
            return result;
        }

        public override Task<RepositoryResponse<bool>> SaveSubModelsAsync(SioTemplate parent, SioCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            TemplateRepository.Instance.SaveTemplate(new TemplateViewModel()
            {
                Filename = FileName,
                Extension = Extension,
                Content = Content,
                FileFolder = FileFolder
            });
            return base.SaveSubModelsAsync(parent, _context, _transaction);
        }

        #endregion Async

        #endregion Overrides

        #region Expands


        /// <summary>
        /// Gets the template by path.
        /// </summary>
        /// <param name="path">The path.</param> Ex: "Pages/_Home"
        /// <returns></returns>
        public static RepositoryResponse<UpdateViewModel> GetTemplateByPath(string path, string culture
            , SioCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            RepositoryResponse<UpdateViewModel> result = new RepositoryResponse<UpdateViewModel>();
            string[] temp = path.Split('/');
            if (temp.Length < 2)
            {
                result.IsSucceed = false;
                result.Errors.Add("Template Not Found");
            }
            else
            {
                int activeThemeId = SioService.GetConfig<int>(
                    SioConstants.ConfigurationKeyword.ThemeId, culture);

                result = Repository.GetSingleModel(t => t.FolderType == temp[0] && t.FileName == temp[1].Split('.')[0] && t.ThemeId == activeThemeId
                    , _context, _transaction);
            }
            return result;
        }

        public static UpdateViewModel GetTemplateByPath(string path, string specificulture, SioEnums.EnumTemplateFolder folderType, SioCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            string templateName = path?.Split('/')[1];
            int themeId = SioService.GetConfig<int>(SioConstants.ConfigurationKeyword.ThemeId, specificulture);
            string themeName = SioService.GetConfig<string>(SioConstants.ConfigurationKeyword.ThemeName, specificulture);
            var getView = UpdateViewModel.Repository.GetSingleModel(t =>
                    t.ThemeId == themeId && t.FolderType == folderType.ToString()
                    && !string.IsNullOrEmpty(templateName) && templateName.Equals($"{t.FileName}{t.Extension}"), _context, _transaction);
            return getView.Data ?? GetDefault(folderType, specificulture);
        }

        public static UpdateViewModel GetDefault(SioEnums.EnumTemplateFolder folderType, string specificulture)
        {
            string activedTheme = SioService.GetConfig<string>(SioConstants.ConfigurationKeyword.ThemeName, specificulture)
                    ?? SioService.GetConfig<string>(SioConstants.ConfigurationKeyword.DefaultTheme);
            string folder = CommonHelper.GetFullPath(new string[]
                    {
                    SioConstants.Folder.TemplatesFolder
                    , activedTheme
                    , folderType.ToString()
                    });
            var defaulTemplate = SioTemplates.UpdateViewModel.Repository.GetModelListBy(
                t => t.Theme.Name == activedTheme && t.FolderType == folderType.ToString()).Data?.FirstOrDefault();
            return defaulTemplate ?? new UpdateViewModel(new SioTemplate()
            {
                ThemeId = SioService.GetConfig<int>(SioConstants.ConfigurationKeyword.ThemeId, specificulture),
                ThemeName = SioService.GetConfig<string>(SioConstants.ConfigurationKeyword.ThemeFolder, specificulture),
                FileName = SioService.GetConfig<string>(SioConstants.ConfigurationKeyword.DefaultTemplate),
                Extension = SioService.GetConfig<string>(SioConstants.ConfigurationKeyword.TemplateExtension),
                Content = SioService.GetConfig<string>(SioConstants.ConfigurationKeyword.DefaultTemplateContent),
                FolderType = folderType.ToString(),
                FileFolder = folder.ToString()
            });

        }
        #endregion
    }
}
