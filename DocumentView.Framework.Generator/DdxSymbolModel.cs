using System.Collections.Generic;

namespace DocumentView.Framework.Generator;

internal sealed record DdxClassModel(
    string Namespace,
    string ClassName,
    IReadOnlyList<DdxMemberModel> Members
);

internal sealed record DdxMemberModel(
    string MemberName,
    string MemberTypeFullName,   // e.g. "string", "int", "System.ComponentModel.BindingList<DocumentView.Sample.ItemRow>"
    bool   CanWrite,
    string ControlName,
    string? ControlProperty,
    IReadOnlyList<DdxValidatorModel> Validators
);

internal sealed record DdxValidatorModel(
    string AttributeTypeFullName,    // e.g. "DocumentView.Framework.DDVMinMaxAttribute"
    IReadOnlyList<string> CtorArgs,  // serialized constructor arguments
    string? MessageValue             // init property value, null if not set
);
