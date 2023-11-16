import App from './App.vue'

describe('<App />', () => {
  it('ensure execution', () => {
    // see: https://on.cypress.io/mounting-vue
    cy.mount(App)
    cy.get('[id=main]').should('have.text', "Hello")
  })
})