﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.OData.Query;
using Swastika.Cms.Lib.Models;
using Swastika.Cms.Lib.ViewModels;
using Swastika.Common.Helper;
using Swastika.Domain.Core.Models;
using Swastika.IO.Cms.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Swastka.Cms.Api.Controllers
{
    [Route("api/{culture}/[controller]")]
    public class ArticlesController :
        BaseAPIController<SiocCmsContext, SiocArticle>
    {
        // POST api/articles
        [HttpPost]
        public async Task<RepositoryResponse<ArticleBEViewModel>> Post([FromBody]ArticleBEViewModel model)
        {
            return await model.SaveModelAsync(true);
        }

        // GET api/articles/id
        [HttpGet]
        [Route("{id}")]
        public async Task<RepositoryResponse<FEArticleViewModel>> Details(string id)
        {
            return await FEArticleViewModel.Repository.GetSingleModelAsync(model => model.Id == id && model.Specificulture == _lang); //base.GetAsync(model => model.Id == id);
        }

        // GET api/articles/id
        [HttpGet]
        [Route("edit/{id}")]
        public async Task<RepositoryResponse<ArticleBEViewModel>> Edit(string id)
        {
            return await ArticleBEViewModel.Repository.GetSingleModelAsync(model => model.Id == id && model.Specificulture == _lang); //base.GetAsync(model => model.Id == id);
        }

        // GET api/articles/id
        [HttpGet]
        [Route("create")]
        public RepositoryResponse<ArticleBEViewModel> Create()
        {
            SiocArticle article = new SiocArticle()
            {
                //Id = Guid.NewGuid().ToString(),
                Specificulture = _lang
            };
            return new RepositoryResponse<ArticleBEViewModel>()
            {
                IsSucceed = true,
                Data = new ArticleBEViewModel(article)
            };
        }


        // GET api/articles/id
        [HttpGet]
        [Route("delete/{id}")]
        public async Task<RepositoryResponse<bool>> Delete(string id)
        {
            var getArticle = ArticleListItemViewModel.Repository.GetSingleModel(a => a.Id == id);
            if (getArticle.IsSucceed)
            {
                return await getArticle.Data.RemoveModelAsync(true);
            }
            else
            {
                return new RepositoryResponse<bool>() { IsSucceed = false };
            }
        }

        // GET api/articles
        [HttpGet]
        [Route("")]
        [Route("{pageSize:int?}/{pageIndex:int?}")]
        [Route("{orderBy}/{direction}")]
        [Route("{pageSize:int?}/{pageIndex:int?}/{orderBy}/{direction}")]
        public async Task<RepositoryResponse<PaginationModel<ArticleListItemViewModel>>> Get(
            int? pageSize = 15, int? pageIndex = 0, string orderBy = "Id"
            , OrderByDirection direction = OrderByDirection.Ascending)
        {
            var data = await ArticleListItemViewModel.Repository.GetModelListByAsync(m => m.Specificulture == _lang, orderBy, direction, pageSize, pageIndex); //base.Get(orderBy, direction, pageSize, pageIndex);
            string domain = string.Format("{0}://{1}", Request.Scheme, Request.Host);
            data.Data.Items.ForEach(d => d.DetailsUrl = string.Format("{0}{1}", domain, this.Url.Action("Details", "articles", new { id = d.Id })));
            data.Data.Items.ForEach(d => d.EditUrl = string.Format("{0}{1}", domain, this.Url.Action("Edit", "articles", new { id = d.Id })));
            return data;
        }

        // GET api/articles
        [HttpGet]
        [Route("{keyword}")]
        [Route("{pageSize:int?}/{pageIndex:int?}/{keyword}")]
        [Route("{pageSize:int?}/{pageIndex:int?}/{orderBy}/{direction}/{keyword}")]
        public async Task<RepositoryResponse<PaginationModel<ArticleListItemViewModel>>> Search(
            string keyword = null, int? pageSize = null, int? pageIndex = null, string orderBy = "Id"
            , OrderByDirection direction = OrderByDirection.Ascending)
        {
            Expression<Func<SiocArticle, bool>> predicate = model =>
            model.Specificulture == _lang &&
            (
            string.IsNullOrWhiteSpace(keyword)
                || (model.Title.Contains(keyword) || model.Content.Contains(keyword))
                );
            return await ArticleListItemViewModel.Repository.GetModelListByAsync(predicate, orderBy, direction, pageSize, pageIndex); // base.Search(predicate, orderBy, direction, pageSize, pageIndex, keyword);
        }

    }
}
