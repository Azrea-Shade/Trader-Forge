use strict; use warnings; local $/ = undef;
my $f = shift or die "file?";
open my $fh, "<", $f or die $!; my $src = <$fh>; close $fh;

# add usings
$src = "using Dapper;\n$src"      unless $src =~ /using\s+Dapper\s*;/;
$src = "using System.Data;\n$src" unless $src =~ /using\s+System\.Data\s*;/;

# add EnsureSchema + guard once inside class
if ($src !~ /EnsureSchema\s*\(IDbConnection\s*db\)/) {
  $src =~ s/(class\s+PortfoliosRepository[^{]*\{)/
$1

    private bool _schemaEnsured;

    private void EnsureSchema(IDbConnection db)
    {
        if (_schemaEnsured) return;
        db.Execute("PRAGMA foreign_keys=ON;");
        db.Execute(@"
CREATE TABLE IF NOT EXISTS portfolios(
  id     INTEGER PRIMARY KEY AUTOINCREMENT,
  name   TEXT NOT NULL,
  notes  TEXT
);
CREATE TABLE IF NOT EXISTS portfolio_holdings(
  id           INTEGER PRIMARY KEY AUTOINCREMENT,
  portfolio_id INTEGER NOT NULL REFERENCES portfolios(id) ON DELETE CASCADE,
  ticker       TEXT NOT NULL,
  weight       REAL NOT NULL DEFAULT 0,
  shares       REAL NULL,
  UNIQUE(portfolio_id, ticker)
);
");
        _schemaEnsured = true;
    }

  /;
}

# call EnsureSchema(db) after each "using var db = ...;"
$src =~ s/(\busing\s+var\s+db\s*=.*?;\s*)(?!\s*EnsureSchema\()/$1\n        EnsureSchema(db);\n/sg;

open my $out, ">", $f or die $!; print $out $src; close $out;
