// Godot Shader Language (GLSL-like)
shader_type canvas_item;

// --- 可调节参数 ---
group_uniforms outline_settings; // 将描边相关参数分组，更整洁
uniform bool enable_outline = false; // 描边开关，默认为false
uniform vec4 outline_color : source_color = vec4(0.0, 0.0, 0.0, 1.0); // 描边颜色
uniform float outline_thickness : hint_range(0.0, 0.5, 0.001) = 0.05; // 描边厚度

group_uniforms shape_settings; // 形状参数分组
uniform float radius : hint_range(0.0, 0.5, 0.001) = 0.15;           // 圆角半径
uniform float feather : hint_range(0.0, 0.01, 0.0001) = 0.005;      // 抗锯齿/羽化宽度

// SDF 函数：计算点 p 到一个大小为 size，圆角为 r 的圆角矩形的距离
float sdf_rounded_box(vec2 p, vec2 size, float r) {
    vec2 q = abs(p) - size + vec2(r);
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

void fragment() {
    // --- 1. 准备工作：校正坐标系以适应宽高比 ---
    float aspect = TEXTURE_PIXEL_SIZE.y / TEXTURE_PIXEL_SIZE.x;
    vec2 st = UV;
    st.x *= aspect;
    vec2 center = vec2(aspect * 0.5, 0.5);
    vec2 size = vec2(aspect * 0.5, 0.5);
    vec2 p = st - center;
    
    // --- 2. 安全性检查与开关逻辑 (核心改动) ---
    
    // a) 根据开关决定有效的描边厚度
    // float(true) -> 1.0, float(false) -> 0.0
    float effective_outline = outline_thickness * float(enable_outline);

    // b) 安全性检查
    float clamped_radius = min(radius, min(size.x, size.y));
    float clamped_outline = min(effective_outline, clamped_radius);

    // --- 3. 计算 SDF 距离 ---
    float dist = sdf_rounded_box(p, size, clamped_radius);

    // --- 4. 定义内容区域和整体形状区域 ---
    float alpha_shape = smoothstep(feather, -feather, dist);
    float alpha_content = smoothstep(-clamped_outline + feather, -clamped_outline - feather, dist);

    // --- 5. 混合颜色并输出 ---
    vec4 tex_color = texture(TEXTURE, UV);
    vec4 final_color = mix(outline_color, tex_color, alpha_content);
    COLOR = vec4(final_color.rgb, alpha_shape * final_color.a);
}
