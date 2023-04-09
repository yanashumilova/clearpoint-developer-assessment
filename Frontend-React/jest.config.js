module.exports = {
  transformIgnorePatterns: ['node_modules/(?!@axios)/'],
  testEnvironment: 'jsdom',
  setupFilesAfterEnv: ['./src/setupTests.js'],
}
