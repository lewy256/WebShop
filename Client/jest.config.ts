import type {Config} from 'jest';
const config: Config = {
  clearMocks: true,
  collectCoverage: true,
  coverageDirectory: "jest-coverage",
  coverageReporters: [
    "text",
    "lcov",
    "json",
    'cobertura'
  ],
  preset: 'jest-preset-angular',
  reporters: [
    "default", "summary",
    ["jest-junit", {outputDirectory: 'results', outputName: 'unit-tests-results.xml'}]
  ],
  setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
}
export default config;
