using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class ValveGenerator : IIncrementalGenerator
{
  private const string AttributeFullName = "RegisterValveAttribute";

  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
	// 1. 寻找所有被我们的特性标记的类，并提取所需信息
	IncrementalValuesProvider<ValveData> classProvider = context.SyntaxProvider
	  .ForAttributeWithMetadataName(
	  AttributeFullName,
	  predicate: (node, _) => node is ClassDeclarationSyntax,
	  // 关键的转换逻辑在这里！
	  transform: (ctx, _) =>
	  {
		var classSymbol = (INamedTypeSymbol)ctx.TargetSymbol;

		// 找到应用到这个类上的我们的特性
		AttributeData attributeData = classSymbol.GetAttributes().Single(ad => ad.AttributeClass?.ToDisplayString() == AttributeFullName);

		return new ValveData(
	  (string)attributeData.ConstructorArguments[0].Value,
	  (ValveTypes)attributeData.ConstructorArguments[1].Value,
	  (bool)attributeData.ConstructorArguments[2].Value
	  );
	  })
	  .Where(m => m is not null)!;

	// 2. 将找到的类信息与编译上下文结合
	var compilationAndClasses = context.CompilationProvider.Combine(classProvider.Collect());

	// 3. 注册最终的生成动作
	context.RegisterSourceOutput(compilationAndClasses,
	  (spc, source) => Execute(source.Right, spc));
  }

  private void Execute(ImmutableArray<ValveData> classes, SourceProductionContext context)
  {
	if (classes.IsDefaultOrEmpty)
	{
	  return;
	}
	foreach (var cls in classes)
	{
	  if (cls.ValveType == ValveTypes.STATE)
	  {
		StringBuilder builder = new StringBuilder();
		builder.Append($"public partial class {cls.Action}StateValve(IPieceState pieceState, {cls.Action}Event @event) : StateValve(pieceState, @event) {{\n");
		builder.Append("  protected override void DoLaunch()\n");
		builder.Append("  {\n");
		builder.Append($"    _pieceState.Query<I{AdjectiveConverter.ToAbleAdjective(cls.Action)}>().ReciveEvent(@event);\n");
		if (cls.EventReciveable)
		{
		  builder.Append("    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);\n");
		}
		builder.Append("  }\n");
		builder.Append("}");
		context.AddSource($"{cls.Action}StateValve.g.cs", builder.ToString());
	  }
	  else
	  {
		StringBuilder builder = new StringBuilder();
		builder.Append($"public partial class {cls.Action}InstanceValve(IPieceInstance pieceInstance, {cls.Action}Event @event) : InstanceValve(pieceInstance, @event) {{\n");
		builder.Append("  protected override void DoLaunch()\n");
		builder.Append("  {\n");
		builder.Append($"    _pieceInstance.Query<I{AdjectiveConverter.ToAbleAdjective(cls.Action)}>().ReciveEvent(@event);\n");
		builder.Append("  }\n");
		builder.Append("}");
		context.AddSource($"{cls.Action}InstanceValve.g.cs", builder.ToString());
	  }
	}
  }
}

namespace System.Runtime.CompilerServices
{
  internal static class IsExternalInit { }
}
