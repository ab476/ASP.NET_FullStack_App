import { registerOTel } from "@vercel/otel";
import { PeriodicExportingMetricReader } from "@opentelemetry/sdk-metrics";
import { OTLPMetricExporter } from "@opentelemetry/exporter-metrics-otlp-proto";
export function register() {
  console.log("Registering OpenTelemetry instrumentation...");

  registerOTel({
    serviceName: "next-app",
    metricReaders: [
      new PeriodicExportingMetricReader({ exporter: new OTLPMetricExporter() }),
    ],
  });
}
