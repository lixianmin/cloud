#!/usr/bin/ruby

# refer: http://mypages.iit.edu/~wzhan/?p=48

require 'test/unit'

def words text
  text.downcase.scan(/[a-z]+/)
end

def train features
  model = Hash.new(1)
  features.each {|f| model[f] += 1 }
  return model
end

NWORDS = train(words(open('big.txt').read))
LETTERS = 'abcdefghijklmnopqrstuvwxyz'

def edits1 word
  n = word.length

  deletion = (0...n).collect {|i| word[0..i-1] + word[i+1..-1] }
  transposition = (0...n-1).collect { |i| word[0..i-1] + word[i+1] + word[i] + word[i+2..-1] }

  alteration = []
  n.times {|i| LETTERS.each_byte { |l| alteration << word[0...i]+ l.chr + word[i+1..-1] } }

  insertion = []
  (n+1).times {|i| LETTERS.each_byte { |l| insertion << word[0...i]+l.chr+word[i..-1] } }

  result = deletion + transposition + alteration + insertion
  result.empty? ? nil : result
end

def known_edits2 word
  result = []
  edits1(word).each do |e1|
      edits1(e1).each do |e2|
          result << e2 if NWORDS.has_key?(e2)
      end
  end
  result.empty? ? nil : result
end

def known words
  result = words.select {|w| NWORDS.has_key?(w) }
  result.empty? ? nil : result
end

def correct word
  (known([word]) or known(edits1(word)) or known_edits2(word) or [word]).max {|a,b| NWORDS[a] <=> NWORDS[b] }
end

class TestUnitAssertionsTest < Test::Unit::TestCase
  def setup
  end

  def test_assertions
    assert_equal 'small', correct('small')
    assert_equal 'economic', correct('economtric')
    assert_equal 'hello', correct('hlelo')
  end

  def teardown
  end
end

