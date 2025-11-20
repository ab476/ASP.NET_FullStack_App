"use client";

import * as d3 from "d3";
import { useEffect, useRef } from "react";

export default function ResponsiveChart() {
  const ref = useRef<SVGSVGElement | null>(null);

  useEffect(() => {
    function renderChart() {
      const container = ref.current?.parentElement;
      if (!container) return;

      const width = container.clientWidth;
      const height = 400;

      const margin = { top: 20, right: 20, bottom: 30, left: 40 };

      // Clear old content
      d3.select(ref.current).selectAll("*").remove();

      // Scales
      const x = d3
        .scaleUtc()
        .domain([new Date("2023-01-01"), new Date("2024-01-01")])
        .range([margin.left, width - margin.right]);

      const y = d3
        .scaleLinear()
        .domain([0, 100])
        .range([height - margin.bottom, margin.top]);

      // Axes
      const svg = d3
        .select(ref.current)
        .attr("width", width)
        .attr("height", height);

      svg
        .append("g")
        .attr("transform", `translate(0,${height - margin.bottom})`)
        .call(d3.axisBottom(x));

      svg
        .append("g")
        .attr("transform", `translate(${margin.left},0)`)
        .call(d3.axisLeft(y));
    }

    renderChart();
    window.addEventListener("resize", renderChart);
    return () => window.removeEventListener("resize", renderChart);
  }, [ref]);

  return <svg ref={ref} />;
}

