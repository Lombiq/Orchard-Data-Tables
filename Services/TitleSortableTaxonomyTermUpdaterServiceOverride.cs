using Lombiq.DataTables.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using Piedone.HelpfulLibraries.Libraries.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.DataTables.Services
{
    public class TitleSortableTaxonomyTermUpdaterServiceDecoratorsModule : DecoratorsModuleBase
    {
        protected override IEnumerable<DecorationConfiguration> DescribeDecorators()
        {
            yield return DecorationConfiguration.Create<ITaxonomyService, TitleSortableTaxonomyTermUpdaterServiceDecorator>();
        }
    }


    internal class TitleSortableTaxonomyTermUpdaterServiceDecorator : ITaxonomyService
    {
        private readonly ITaxonomyService _decorated;


        public TitleSortableTaxonomyTermUpdaterServiceDecorator(ITaxonomyService decorated)
        {
            _decorated = decorated;
        }


        public void CreateHierarchy(IEnumerable<TermPart> terms, Action<TermPartNode, TermPartNode> append)
        {
            _decorated.CreateHierarchy(terms, append);
        }

        public void CreateTermContentType(TaxonomyPart taxonomy)
        {
            _decorated.CreateTermContentType(taxonomy);
        }

        public void DeleteTaxonomy(TaxonomyPart taxonomy)
        {
            _decorated.DeleteTaxonomy(taxonomy);
        }

        public void DeleteTerm(TermPart termPart)
        {
            _decorated.DeleteTerm(termPart);
        }

        public string GenerateTermTypeName(string taxonomyName) =>
            _decorated.GenerateTermTypeName(taxonomyName);

        public IEnumerable<TermPart> GetChildren(TermPart term) =>
            _decorated.GetChildren(term);

        public IEnumerable<TermPart> GetChildren(TermPart term, bool includeParent) =>
            _decorated.GetChildren(term, includeParent);

        public IEnumerable<IContent> GetContentItems(TermPart term, int skip = 0, int count = 0, string fieldName = null) =>
            _decorated.GetContentItems(term, skip, count, fieldName);

        public long GetContentItemsCount(TermPart term, string fieldName = null) =>
            _decorated.GetContentItemsCount(term, fieldName);

        public IContentQuery<TermsPart, TermsPartRecord> GetContentItemsQuery(TermPart term, string fieldName = null) =>
            _decorated.GetContentItemsQuery(term, fieldName);

        public IEnumerable<TermPart> GetParents(TermPart term) =>
            _decorated.GetParents(term);

        public IEnumerable<TermPart> GetRootTerms(int taxonomyId) =>
            _decorated.GetRootTerms(taxonomyId);

        public IEnumerable<TaxonomyPart> GetTaxonomies() =>
            _decorated.GetTaxonomies();

        public IContentQuery<TaxonomyPart, TaxonomyPartRecord> GetTaxonomiesQuery() =>
            _decorated.GetTaxonomiesQuery();

        public TaxonomyPart GetTaxonomy(int id) =>
            _decorated.GetTaxonomy(id);

        public TaxonomyPart GetTaxonomyByName(string name) =>
            _decorated.GetTaxonomyByName(name);

        public TermPart GetTerm(int id) =>
            _decorated.GetTerm(id);

        public TermPart GetTermByName(int taxonomyId, string name) =>
            _decorated.GetTermByName(taxonomyId, name);

        public IEnumerable<TermPart> GetTerms(int taxonomyId) =>
            _decorated.GetTerms(taxonomyId);

        public int GetTermsCount(int taxonomyId) =>
            _decorated.GetTermsCount(taxonomyId);

        public IEnumerable<TermPart> GetTermsForContentItem(int contentItemId, string field = null, VersionOptions versionOptions = null) =>
            _decorated.GetTermsForContentItem(contentItemId, field, versionOptions);

        public IContentQuery<TermPart, TermPartRecord> GetTermsQuery(int taxonomyId) =>
            _decorated.GetTermsQuery(taxonomyId);

        public void MoveTerm(TaxonomyPart taxonomy, TermPart term, TermPart parentTerm)
        {
            _decorated.MoveTerm(taxonomy, term, parentTerm);
        }

        public TermPart NewTerm(TaxonomyPart taxonomy) =>
            _decorated.NewTerm(taxonomy);

        public TermPart NewTerm(TaxonomyPart taxonomy, IContent parent) =>
            _decorated.NewTerm(taxonomy, parent);

        public string ProcessChildrenFullWeight(string childrenFullWeight, string parentFullWeight, string parentOldFullWeight) =>
            _decorated.ProcessChildrenFullWeight(childrenFullWeight, parentFullWeight, parentOldFullWeight);

        public void ProcessFullWeight(TermPart term, TermPart parentTerm) =>
            _decorated.ProcessFullWeight(term, parentTerm);

        public void ProcessPath(TermPart term) =>
            _decorated.ProcessPath(term);

        public void UpdateTerms(ContentItem contentItem, IEnumerable<TermPart> terms, string field)
        {
            _decorated.UpdateTerms(contentItem, terms, field);

            var titleSortableTermsPart = contentItem.As<TitleSortableTermsPart>();

            // Clearing current Terms.
            var termList = titleSortableTermsPart.TermParts.Select((t, i) => new { Term = t, Index = i })
                .Where(x => x.Term.Field == field)
                .Select(x => x)
                .OrderByDescending(i => i.Index)
                .ToList();

            foreach (var x in termList) titleSortableTermsPart.TermParts.RemoveAt(x.Index);

            // Re-adding selected Terms.
            foreach (var term in terms)
            {
                // Remove the newly added terms because they will get processed by the Published-Event.
                termList.RemoveAll(t => t.Term.Id == term.Id);

                titleSortableTermsPart.TermParts.Add(
                    new TitleSortableTermContentItem
                    {
                        TitleSortableTermsPartRecord = titleSortableTermsPart.Record,
                        TermRecord = term.Record,
                        TitlePartRecord = term.As<TitlePart>().Record,
                        Field = field
                    });
            }
        }
    }
}