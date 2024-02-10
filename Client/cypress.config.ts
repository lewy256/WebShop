import { defineConfig } from 'cypress'

export default defineConfig({

  e2e: {
    setupNodeEvents(on, config) {
      require('@cypress/code-coverage/task')(on, config)
      // ...
      return config
    }
  },
  reporter: 'junit',
  reporterOptions: {
    mochaFile: 'results/e2e-tests-results.xml',
    toConsole: true
  },

  component: {
    devServer: {
      framework: 'angular',
      bundler: 'webpack',
    },
    specPattern: '**/*.cy.ts'
  }

})
