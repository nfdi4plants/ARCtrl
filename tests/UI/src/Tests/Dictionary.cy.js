import Dictionary from './Dictionary.vue'

describe('<Dictionary />', () => {
    it('renders Dictionary',() => {
        cy.mount(Dictionary)
        cy.get('button[id=ReactivityButton]').should('have.text', "Dictionary: 121")
    })
    it('updates Dictionary',() => {
        cy.mount(Dictionary)
        cy.get('button[id=ReactivityButton]').click()
        cy.get('button[id=ReactivityButton]', {timeout : 500} ).should('have.text', "Dictionary: 120")
        cy.get('button[id=ReactivityButton]').click()
        cy.get('button[id=ReactivityButton]').should('have.text', "Dictionary: 119")
    })
    it('renders ResizeArray',() => {
        cy.mount(Dictionary)
        cy.get('button[id=ReactivityButtonArr]').should('have.text', "ResizeArray: 121")
    })
    it('updates ResizeArray',() => {
        cy.mount(Dictionary)
        cy.get('button[id=ReactivityButtonArr]').click()
        cy.get('button[id=ReactivityButtonArr]', {timeout : 500} ).should('have.text', "ResizeArray: 120")
        cy.get('button[id=ReactivityButtonArr]').click()
        cy.get('button[id=ReactivityButtonArr]').should('have.text', "ResizeArray: 119")
    })
    it('renders ArcTable', () => {
        cy.mount(Dictionary)
        cy.get("button[id=ArcTableReactivityButton]").should('have.text', "ArcTable Rows: 10")
    })
    it('updates ArcTable', () => {
        cy.mount(Dictionary)
        cy.get("button[id=ArcTableReactivityButton]").click()
        cy.get("button[id=ArcTableReactivityButton]", {timeout : 500} ).should('have.text', "ArcTable Rows: 9")
        cy.get("button[id=ArcTableReactivityButton]").click()
        cy.get("button[id=ArcTableReactivityButton]").should('have.text', "ArcTable Rows: 8")
    })
})