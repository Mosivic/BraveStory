import yaml
from typing import Dict, List, Any

class AttributeDefinition:
    def __init__(self, name: str, value: float = 0):
        self.name = name
        self.value = value
        self.calculate_mode = "Stacking"
        self.min_value = float('-inf')
        self.max_value = float('inf')

class AttributeSetDefinition:
    def __init__(self, name: str):
        self.name = name
        self.inherits: List[str] = []
        self.attributes: Dict[str, AttributeDefinition] = {}



class AttributeGenerator:
    def __init__(self):
        self.attribute_sets: Dict[str, AttributeSetDefinition] = {}
    
    def load_yaml(self, yaml_path: str):
        with open(yaml_path, 'r', encoding='utf-8') as f:
            data = yaml.safe_load(f)
            
        for set_name, set_data in data.items():
            attr_set = AttributeSetDefinition(set_name)
            
            if isinstance(set_data, list):
                for item in set_data:
                    if not isinstance(item, dict):
                        continue
                        
                    # 处理继承
                    if 'inherits' in item:
                        attr_set.inherits.append(item['inherits'])
                        
                    # 处理属性
                    if 'properties' in item:
                        properties = item['properties']
                        if isinstance(properties, dict):
                            for attr_name, attr_value in properties.items():
                                if attr_name == 'Name':  # 跳过名称属性
                                    continue
                                # 跳过向量值
                                if isinstance(attr_value, list):
                                    continue
                                try:
                                    attr_def = AttributeDefinition(attr_name, float(attr_value))
                                    attr_set.attributes[attr_name] = attr_def
                                except (ValueError, TypeError):
                                    print(f"警告: 无法转换属性值 {attr_name}: {attr_value} 为浮点数，已跳过")
            
            if attr_set.attributes or attr_set.inherits:  # 只添加有效的属性集
                self.attribute_sets[set_name] = attr_set
    
    def resolve_inheritance(self):
        resolved_sets = {}
        
        def resolve_set(set_name: str) -> Dict[str, AttributeDefinition]:
            # 如果已经解析过，直接返回
            if set_name in resolved_sets:
                return resolved_sets[set_name].copy()
            
            if set_name not in self.attribute_sets:
                print(f"警告: 找不到继承的属性集 {set_name}")
                return {}
            
            attr_set = self.attribute_sets[set_name]
            final_attributes = {}
            
            # 首先解析所有继承的属性
            for inherit in attr_set.inherits:
                inherited_attrs = resolve_set(inherit)
                # 合并继承的属性
                for name, attr in inherited_attrs.items():
                    final_attributes[name] = AttributeDefinition(name, attr.value)
            
            # 然后用自己的属性覆盖继承的属性
            for name, attr in attr_set.attributes.items():
                final_attributes[name] = AttributeDefinition(name, attr.value)
                
            # 缓存解析结果
            resolved_sets[set_name] = final_attributes
            return final_attributes.copy()
            
        # 解析所有属性集
        for set_name in list(self.attribute_sets.keys()):
            if set_name not in resolved_sets:
                self.attribute_sets[set_name].attributes = resolve_set(set_name)

    def generate_tags_class(self) -> str:
        code_parts = []
        code_parts.append("public static partial class Tags\n")
        code_parts.append("{\n")

        generated_tags = set()
        
        # 为每个 AttributeSet 生成标签
        for set_name in self.attribute_sets.keys():
            tag_identifier = f"AttributeSet.{set_name}"
            if tag_identifier not in generated_tags:
                code_parts.append(f"    public static Tag AttributeSet_{set_name} {{ get; }} = TagManager.RequestTag(\"{tag_identifier}\");\n")
                generated_tags.add(tag_identifier)  # 添加到已生成标签集合
        

        for attr_set in self.attribute_sets.values():
            for attr_name in attr_set.attributes.keys():
                tag_identifier = f"Attribute.{attr_name}"
                if tag_identifier not in generated_tags:
                    code_parts.append(f"    public static Tag Attribute_{attr_name} {{ get; }} = TagManager.RequestTag(\"{tag_identifier}\");\n")
                    generated_tags.add(tag_identifier)
        
        code_parts.append("}\n")
        return "".join(code_parts)

    def generate_code(self) -> str:
        code_parts = []
        code_parts.append("// This file is auto-generated. Do not modify.\n")
        code_parts.append("namespace Miros.Core;\n")
        
        # 生成标签类
        code_parts.append(self.generate_tags_class())
        
        # 生成基类
        for set_name, attr_set in self.attribute_sets.items():
            if not attr_set.inherits:  # 没有继承的是基类
                code_parts.extend(self._generate_attribute_set(set_name, attr_set))
        
        # 然后生成继承类
        for set_name, attr_set in self.attribute_sets.items():
            if attr_set.inherits:  # 有继承的类
                parent_class = f"{attr_set.inherits[0]}AttributeSet"  # 取第一个父类
                code_parts.extend(self._generate_attribute_set(set_name, attr_set, parent_class))
        
        return "\n".join(code_parts)

    def _generate_attribute_set(self, set_name: str, attr_set: AttributeSetDefinition, parent_class: str = "AttributeSet") -> List[str]:
        code_parts = []
        code_parts.append(f"public class {set_name}AttributeSet : {parent_class}")
        code_parts.append("{")
        
        # 只声明新增的属性字段
        inherited_attrs = set()
        if parent_class != "AttributeSet":
            parent_set = self.attribute_sets[parent_class[:-12]]  # 移除"AttributeSet"后缀
            inherited_attrs = set(parent_set.attributes.keys())
        
        # 声明本类特有的属性字段
        for attr_name in attr_set.attributes.keys():
            if attr_name not in inherited_attrs:
                code_parts.append(f"    private readonly AttributeBase _{attr_name.lower()};")
                
        
        # AttributeSigns 属性
        code_parts.append("\n    public override Tag[] AttributeSigns => new[] {")
        names = [f'Tags.Attribute_{attr_name}' for attr_name in attr_set.attributes.keys()]
        code_parts.append(f"        {', '.join(names)}")
        code_parts.append("    };\n")
        
        # 构造函数
        code_parts.append(f"    public {set_name}AttributeSet() : base()")
        code_parts.append("    {")
        # 只初始化新增的属性
        for attr_name, attr_def in attr_set.attributes.items():
            if attr_name not in inherited_attrs:
                code_parts.append(f'        _{attr_name.lower()} = new AttributeBase(Tags.AttributeSet_{set_name}, Tags.Attribute_{attr_name}, {attr_def.value}f);')
        code_parts.append("    }\n")
        
        # 索引器
        if parent_class == "AttributeSet":
            code_parts.append("    public override AttributeBase this[Tag sign] =>")
            for attr_name in attr_set.attributes.keys():
                code_parts.append(f'        sign.Equals(Tags.Attribute_{attr_name}) ? _{attr_name.lower()} :')
            code_parts.append("        null;\n")
        else:  # 为继承的类添加索引器
            code_parts.append("    public override AttributeBase this[Tag sign] =>")
            # 只处理当前类特有的属性
            for attr_name in attr_set.attributes.keys():
                if attr_name not in inherited_attrs:  # 确保不重复父类属性
                    code_parts.append(f'        sign.Equals(Tags.Attribute_{attr_name}) ? _{attr_name.lower()} :')
            code_parts.append("        base[sign];\n")  # 调用父类的索引器
        
        # 属性访问器（只为新增的属性生成）
        for attr_name in attr_set.attributes.keys():
            if attr_name not in inherited_attrs:
                code_parts.append(f"    public AttributeBase {attr_name} => _{attr_name.lower()};")
        
        code_parts.append("}\n")
        return code_parts

def main():
    generator = AttributeGenerator()
    generator.load_yaml("data/attributes/character.yaml")
    generator.resolve_inheritance()
    
    code = generator.generate_code()
    with open("src/Example/CustomAttributeSets.cs", "w", encoding="utf-8") as f:
        f.write(code)

if __name__ == "__main__":
    main() 