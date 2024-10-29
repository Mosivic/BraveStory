import pandas as pd
import yaml
import os

def excel_to_yaml(excel_path, output_dir):
    # 读取Excel文件中的所有sheet
    excel_data = pd.read_excel(excel_path, sheet_name=None)
    
    # 遍历每个sheet
    for sheet_name, df in excel_data.items():
        # 将DataFrame转换为字典列表
        data_list = df.to_dict('records')
        
        # 创建YAML文件名
        yaml_filename = f"{sheet_name.lower()}.yaml"
        yaml_path = os.path.join(output_dir, yaml_filename)
        
        # 确保输出目录存在
        os.makedirs(output_dir, exist_ok=True)
        
        # 写入YAML文件
        with open(yaml_path, 'w', encoding='utf-8') as yaml_file:
            # 添加文件头注释
            yaml_file.write("# This file is auto-generated from Excel. Do not edit directly.\n")
            yaml_file.write(f"# Source: {os.path.basename(excel_path)}, Sheet: {sheet_name}\n\n")
            
            # 将数据写入YAML
            yaml.dump(data_list, yaml_file, allow_unicode=True, sort_keys=False, default_flow_style=False)
        
        print(f"Generated {yaml_path}")

if __name__ == "__main__":
    # 配置输入输出路径
    excel_path = "GameData/characters.xlsx"
    output_dir = "GameData/yaml"
    
    excel_to_yaml(excel_path, output_dir)