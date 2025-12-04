import { NodeSDK } from "@opentelemetry/sdk-node";
import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-http";
import { getNodeAutoInstrumentations } from "@opentelemetry/auto-instrumentations-node";
import {} from "@opentelemetry/sdk-logs"
const traceExporter = new OTLPTraceExporter({
  url: process.env.OTEL_EXPORTER_OTLP_ENDPOINT, // example: http://localhost:4318/v1/traces
});

const sdk = new NodeSDK({
  serviceName: "nextjs-app",
  traceExporter,
  instrumentations: [getNodeAutoInstrumentations()],
});

// Start SDK
sdk.start();
console.log("✅ OpenTelemetry initialized for Next.js");
