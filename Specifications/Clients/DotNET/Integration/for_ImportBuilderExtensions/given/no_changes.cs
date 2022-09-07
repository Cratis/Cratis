// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using Aksio.Cratis.Changes;

namespace Aksio.Cratis.Integration.for_ImportBuilderExtensions.given;

public class no_changes : Specification
{
    protected IImportBuilderFor<Model, ExternalModel> import_builder;
    protected IImportActionBuilderFor<Model, ExternalModel> action_builder;
    protected Subject<ImportContext<Model, ExternalModel>> subject;
    protected IObservable<ImportContext<Model, ExternalModel>> context;
    protected Changeset<Model, Model> changeset;
    protected EventsToAppend events_to_append;
    protected Model original_model;
    protected Model modified_model;
    protected Mock<IObjectsComparer> objects_comparer;

    void Establish()
    {
        subject = new Subject<ImportContext<Model, ExternalModel>>();
        action_builder = new ImportActionBuilderFor<Model, ExternalModel>(subject);
        import_builder = new ImportBuilderFor<Model, ExternalModel>(subject);
        modified_model = new Model(42, "Forty Two");
        original_model = new Model(42, "Forty Two");
        objects_comparer = new();
        objects_comparer.Setup(_ => _.Equals(original_model, modified_model, out Ref<IEnumerable<PropertyDifference>>.IsAny)).Returns(true);
        changeset = new(objects_comparer.Object, modified_model, original_model);
        events_to_append = new();
    }
}
