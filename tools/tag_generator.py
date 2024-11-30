import yaml
import os
from typing import Dict, Any, List, Set
from pathlib import Path

class TagDefinition:
    def __init__(self, name: str, full_path: str, description: str = ""):
        self.name = name
        self.full_path = full_path
        self.description = description
        self.inherits: List[str] = []
        self.children: List[TagDefinition] = []
    
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
            "using Miros.Core;",
            "",
            f"namespace {namespace}",
            "{",
            self._generate_tags_class(),
            "}"
        ])
    
    def _generate_tags_class(self) -> str:
        """生成标签类"""
        lines = [
            f"{self.indent}public static partial class Tags",
            f"{self.indent}{{",
            f"{self.indent}{self.indent}private static TagManager TagManager => TagManager.Instance;",
            ""
        ]
        
        # 生成标签定义，使用完整路径名作为变量名
        for tag in self.tag_loader.tag_trees.values():
            var_name = tag.full_path.replace('.', '_')
            lines.append(
                f"{self.indent}{self.indent}public static Tag {var_name} {{ get; }} = "
                f"TagManager.RequestTag(\"{tag.full_path}\");"
            )
            lines.append("")
        
        lines.append(f"{self.indent}}}")
        return "\n".join(lines)

def main():
    # 配置
    src_dir = "data/tags/"
    output_path = "src/Example/GameplayTags.cs"
    namespace = "BraveStory"
    
    # 加载所有标签文件
    loader = TagTreeLoader()
    loader.load_tag_files(src_dir)
    
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