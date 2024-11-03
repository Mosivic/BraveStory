import yaml
import os
from typing import Dict, Any, List, Set
from pathlib import Path

class TagProperty:
    def __init__(self, name: str, type_name: str, default_value: Any = None):
        self.name = name
        self.type_name = self._normalize_type(type_name)
        self.default_value = default_value
    
    def _normalize_type(self, type_name: str) -> str:
        """标准化类型名称"""
        type_map = {
            'int': 'int',
            'float': 'float',
            'bool': 'bool',
            'string': 'string',
            'double': 'double',
            'vector2': 'Vector2',
            'vector3': 'Vector3'
        }
        return type_map.get(type_name.lower(), type_name)
    
    def get_property_declaration(self) -> str:
        return f"public {self.type_name} {self.name} {{ get; set; }}"
    
    def get_default_value_string(self) -> str:
        """获取默认值的字符串表示"""
        if self.default_value is None:
            return "default"
        if isinstance(self.default_value, str):
            return f'"{self.default_value}"'
        if isinstance(self.default_value, bool):
            return str(self.default_value).lower()
        if self.type_name == "float":  # 为float类型添加F后缀
            return f"{self.default_value}f"
        if self.type_name == "Vector2" and isinstance(self.default_value, list) and len(self.default_value) == 2:
            return f"new Vector2({self.default_value[0]}f, {self.default_value[1]}f)"
        if self.type_name == "Vector3" and isinstance(self.default_value, list) and len(self.default_value) == 3:
            return f"new Vector3({self.default_value[0]}f, {self.default_value[1]}f, {self.default_value[2]}f)"
        return str(self.default_value)

class TagDefinition:
    def __init__(self, name: str, full_path: str, description: str = ""):
        self.name = name
        self.full_path = full_path
        self.description = description
        self.properties: Dict[str, TagProperty] = {}
        self.inherits: List[str] = []
        self.children: List[TagDefinition] = []
    
    def add_property(self, name: str, type_name: str, default_value: Any = None):
        self.properties[name] = TagProperty(name, type_name, default_value)
    
    def add_child(self, child: 'TagDefinition'):
        self.children.append(child)

class TagTreeLoader:
    def __init__(self):
        self.tag_trees: Dict[str, TagDefinition] = {}
        self.processed_files: Set[str] = set()
    
    def load_tag_files(self, root_dir: str):
        """加载所有标签文件"""
        # 命名规则为：“*_tags.yaml", "*_tags.yml"
        for file_path in Path(root_dir).rglob("*.[yY][aA][mM][lL]"):
            if file_path.name.endswith(("_tags.yaml", "_tags.yml")):
                self.load_tag_file(str(file_path))
    
    def load_tag_file(self, file_path: str):
        """加载单个标签文件"""
        if file_path in self.processed_files:
            return
        
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                data = yaml.safe_load(f)
            
            base_path = data.get('basePath', '')
            tags = data.get('tags', [])
            
            for tag_data in tags:
                self._process_tag(tag_data, base_path)
            
            self.processed_files.add(file_path)
            
        except Exception as e:
            print(f"Error loading tag file {file_path}: {e}")
    
    def _process_tag(self, tag_data: Dict, base_path: str, parent_path: str = ""):
        """处理单个标签定义"""
        name = tag_data['name']
        full_path = f"{base_path}.{name}" if base_path else name
        if parent_path:
            full_path = f"{parent_path}.{name}"
        
        tag = TagDefinition(name, full_path, tag_data.get('description', ''))
        
        # 处理属性
        if 'properties' in tag_data:
            for prop_name, prop_data in tag_data['properties'].items():
                if isinstance(prop_data, dict):
                    type_name = prop_data.get('type', 'string')
                    default_value = prop_data.get('default')
                else:
                    type_name = self._infer_type(prop_data)
                    default_value = prop_data
                tag.add_property(prop_name, type_name, default_value)
        
        # 处理继承
        if 'inherits' in tag_data:
            for inherit_path in tag_data['inherits']:
                if '.' not in inherit_path:
                    inherit_path = f"{base_path}.{inherit_path}"
                tag.inherits.append(inherit_path)
        
        # 处理子标签
        if 'children' in tag_data:
            for child_data in tag_data['children']:
                child_tag = self._process_tag(child_data, base_path, full_path)
                tag.add_child(child_tag)
        
        self.tag_trees[full_path] = tag
        return tag
    
    def _infer_type(self, value: Any) -> str:
        """推断属性类型"""
        if isinstance(value, bool):
            return 'bool'
        if isinstance(value, int):
            return 'int'
        if isinstance(value, float):
            return 'float'
        if isinstance(value, str):
            return 'string'
        if isinstance(value, list):
            if len(value) == 2 and all(isinstance(x, (int, float)) for x in value):
                return 'Vector2'
            if len(value) == 3 and all(isinstance(x, (int, float)) for x in value):
                return 'Vector3'
        return 'string'  # 默认为string类型
    
    def resolve_inheritance(self):
        """解析所有标签的继承关系"""
        for tag in self.tag_trees.values():
            self._resolve_tag_inheritance(tag)
    
    def _resolve_tag_inheritance(self, tag: TagDefinition):
        """解析单个标签的继承关系"""
        inherited_properties = {}
        
        # 收集所有继承的属性
        for inherit_path in tag.inherits:
            if inherit_path in self.tag_trees:
                parent_tag = self.tag_trees[inherit_path]
                for name, prop in parent_tag.properties.items():
                    if name not in inherited_properties:
                        inherited_properties[name] = prop
        
        # 合并继承的属性和自身的属性
        final_properties = {}
        final_properties.update(inherited_properties)
        final_properties.update(tag.properties)
        tag.properties = final_properties

class TagCodeGenerator:
    def __init__(self, tag_loader: TagTreeLoader):
        self.tag_loader = tag_loader
        self.indent = "    "
    
    def generate_code(self, namespace: str) -> str:
        """生成完整的代码文件"""
        return "\n".join([
            "// This file is auto-generated. Do not modify.",
            "using Godot;",
            "using System.Runtime.CompilerServices;",
            "",
            f"namespace {namespace}",
            "{",
            self._generate_tag_data_classes(),
            self._generate_tags_class(),
            "}"
        ])
    
    def _generate_tag_data_classes(self) -> str:
        """生成标签数据结构体"""
        definitions = []
        
        # 处理所有包含属性的标签
        for tag in self.tag_loader.tag_trees.values():
            if not tag.properties:
                continue
                
            class_name = self._get_data_struct_name(tag)
            props = {}  # 使用字典来存储属性，确保不重复
            
            # 如果有继承，先添加所有继承的属性
            if tag.inherits:
                for inherit_path in tag.inherits:
                    parent_tag = self.tag_loader.tag_trees[inherit_path]
                    for prop_name, prop in parent_tag.properties.items():
                        if prop_name not in props:  # 只有在属性不存在时才添加
                            props[prop_name] = (
                                f"{self.indent}{self.indent}public {prop.type_name} "
                                f"{prop.name} = {prop.get_default_value_string()};"
                            )
            
            # 添加自己的属性（如果与继承的属性重复，会覆盖）
            for prop_name, prop in tag.properties.items():
                props[prop_name] = (
                    f"{self.indent}{self.indent}public {prop.type_name} "
                    f"{prop.name} = {prop.get_default_value_string()};"
                )
            
            # 将属性列表转换为有序的行
            prop_lines = list(props.values())
            
            definitions.extend([
                f"{self.indent}public class {class_name}",
                f"{self.indent}{{",
                *prop_lines,
                f"{self.indent}}}",
                ""
            ])
        
        return "\n".join(definitions)
    
    def _generate_tags_class(self) -> str:
        """生成标签类"""
        lines = [
            f"{self.indent}public static class Tags",
            f"{self.indent}{{",
            f"{self.indent}{self.indent}private static GameplayTagManager TagManager => GameplayTagManager.Instance;",
            ""
        ]
        
        # 只生成标签定义
        for tag in self.tag_loader.tag_trees.values():
            const_name = self._to_const_name(tag.name)
            lines.append(
                f"{self.indent}{self.indent}public static GameplayTag {const_name} {{ get; }} = "
                f"TagManager.RequestGameplayTag(\"{tag.full_path}\");"
            )
            lines.append("")
        
        lines.append(f"{self.indent}}}")
        return "\n".join(lines)
    
    def _get_data_struct_name(self, tag: TagDefinition) -> str:
        """获取数据结构名称"""
        return f"{self._to_const_name(tag.name)}Data"
    
    def _to_const_name(self, name: str) -> str:
        """转换为常量名称"""
        # return name.upper()
        return name

def main():
    # 配置
    src_dir = "data/"
    output_path = "src/Example/GameplayTags.cs"
    namespace = "BraveStory"
    
    # 加载所有标签文件
    loader = TagTreeLoader()
    loader.load_tag_files(src_dir)
    
    # 解析继承关系
    loader.resolve_inheritance()
    
    # 生成代码
    generator = TagCodeGenerator(loader)
    code = generator.generate_code(namespace)
    
    # 写入文件
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(code)
    print(f"Successfully generated code at: {output_path}")

if __name__ == "__main__":
    main() 